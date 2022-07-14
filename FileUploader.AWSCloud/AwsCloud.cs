
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
using Microsoft.Extensions.Configuration;

namespace FileUploader.AWSCloud
{
    public class AwsCloud : IAwsCloud
    {
        private readonly ISecretManager _secretManager;

        public AwsCloud(ISecretManager secretManager)
        {
            _secretManager = secretManager;
        }
        
        public async Task UploadFile(IFormFile file, AwsCloudProvider awsCloudProvider)
        {
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
                    BucketName = awsCloudProvider.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                // Created an S3 client
                using var client = GetAmazonS3Client(awsCloudProvider);

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

        public async Task<FileResponse> DownloadFile(string fileName, AwsCloudProvider awsCloudProvider)
        {
            try
            {
                // Created an S3 client
                using var client = GetAmazonS3Client(awsCloudProvider);

                var res = await client.GetObjectAsync(new GetObjectRequest()
                {
                    Key = fileName,
                    BucketName = awsCloudProvider.BucketName
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

        public async Task<string> GetShareableUrl(string fileName, AwsCloudProvider awsCloudProvider, int expireInMinutes=15)
        {
            try
            {
                // Created an S3 client
                using var client = GetAmazonS3Client(awsCloudProvider);
                
                if (!IsFileExists(fileName, awsCloudProvider.BucketName, client)) 
                    throw new BadRequestException("File not found");

                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = awsCloudProvider.BucketName,
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

        private AmazonS3Client GetAmazonS3Client(AwsCloudProvider awsCloudProvider)
        {
            var accessKeyId = _secretManager.GetSecret($"{awsCloudProvider.ClientId}-{Shared.Constants.SecretKeys.AwsAccessKeyId}").GetAwaiter().GetResult();
            var secretKey = _secretManager.GetSecret($"{awsCloudProvider.ClientId}-{Shared.Constants.SecretKeys.AwsSecretAccessKey}").GetAwaiter().GetResult();
            
            var credentials = new BasicAWSCredentials(accessKeyId, secretKey);

            // Specify the region
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsCloudProvider.Region)
            };
            
            return new AmazonS3Client(credentials, config);
        }
    }
}