
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FileUploader.Shared;
using FileUploader.Shared.Exceptions;
using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FileUploader.AwsCloudProvider
{
    public class AwsCloudProvider : ICloudProvider
    {
        private readonly ISecretManager _secretManager;

        public AwsCloudProvider(ISecretManager secretManager)
        {
            _secretManager = secretManager;
        }
        
        public async Task UploadFile(IFormFile file, object awsProviderDetails)
        {
            var awsCloudProviderDetails = (awsProviderDetails as AwsCloudProviderDetails);
            await using var memoryStr = new MemoryStream();
            await file.CopyToAsync(memoryStr);
            
            var objName = $"{file.FileName}";
            
            try
            {
                // Create the upload request
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = memoryStr,
                    Key = objName,
                    BucketName = awsCloudProviderDetails.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                // Created an S3 client
                using var client = GetAmazonS3Client(awsCloudProviderDetails);

                // upload utility to s3
                var transferUtiltiy = new TransferUtility(client);

                // We are actually uploading the file to S3
                await transferUtiltiy.UploadAsync(uploadRequest);
                
            }
            catch(Exception)
            {
                throw new Exception($"Unable to upload file");
            }
        }

        public async Task<FileResponse> DownloadFile(string fileName, object awsProviderDetails)
        {
            var awsCloudProviderDetails = (awsProviderDetails as AwsCloudProviderDetails);
            try
            {
                // Created an S3 client
                using var client = GetAmazonS3Client(awsCloudProviderDetails);

                var res = await client.GetObjectAsync(new GetObjectRequest()
                {
                    Key = fileName,
                    BucketName = awsCloudProviderDetails.BucketName
                });

                return new FileResponse
                {
                    FileName = fileName,
                    ResponseStream = res.ResponseStream,
                    ContentType = res.Headers.ContentType
                };
            }
            catch (Exception)
            {
                throw new Exception("File not found");
            }
        }

        public async Task<string> GetShareableUrl(string fileName, object awsProviderDetails, int expireInMinutes=15)
        {
            var awsCloudProviderDetails = (awsProviderDetails as AwsCloudProviderDetails);
            try
            {
                // Created an S3 client
                using var client = GetAmazonS3Client(awsCloudProviderDetails);
                
                if (!IsFileExists(fileName, awsCloudProviderDetails.BucketName, client)) 
                    throw new BadRequestException("File not found");

                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = awsCloudProviderDetails.BucketName,
                    Key = fileName,
                    Expires = DateTime.Now.AddMinutes(expireInMinutes)
                };

                // Get path for request
                return client.GetPreSignedURL(request);
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Exception("Unable to create shareable url");
            }
        }

        private bool IsFileExists(string fileName, string bucketName, AmazonS3Client s3Client)
        {
            var listResponse = s3Client.ListObjectsV2Async(new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = fileName
            }).GetAwaiter().GetResult();

            return listResponse.KeyCount > 0;
        }

        private AmazonS3Client GetAmazonS3Client(AwsCloudProviderDetails awsCloudProviderDetails)
        {
            var accessKeyId = _secretManager.GetSecret($"{awsCloudProviderDetails.ClientId}-{Shared.Constants.SecretKeys.AwsAccessKeyId}").GetAwaiter().GetResult();
            var secretKey = _secretManager.GetSecret($"{awsCloudProviderDetails.ClientId}-{Shared.Constants.SecretKeys.AwsSecretAccessKey}").GetAwaiter().GetResult();
            
            var credentials = new BasicAWSCredentials(accessKeyId, secretKey);

            // Specify the region
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsCloudProviderDetails.Region)
            };
            
            return new AmazonS3Client(credentials, config);
        }
    }
}