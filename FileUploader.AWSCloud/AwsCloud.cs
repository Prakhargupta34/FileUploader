
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FileUploader.Shared;
using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FileUploader.AWSCloud
{
    public class AwsCloud : IAwsCloud
    {
        private readonly IConfiguration _configuration;
        private readonly ISecretManager _secretManager;

        public AwsCloud(IConfiguration configuration, ISecretManager secretManager)
        {
            _configuration = configuration;
            _secretManager = secretManager;
        }
        
        public async Task UploadFile(IFormFile file, AwsCloudProvider awsCloudProvider)
        {
            await using var memoryStr = new MemoryStream();
            await file.CopyToAsync(memoryStr);
            
            var objName = $"{file.FileName}";

            var accessKeyId = _secretManager.GetSecret($"{awsCloudProvider.ClientId}-{Shared.Constants.SecretKeys.AwsAccessKeyId}").GetAwaiter().GetResult();
            var secretKey = _secretManager.GetSecret($"{awsCloudProvider.ClientId}-{Shared.Constants.SecretKeys.AwsSecretAccessKey}").GetAwaiter().GetResult();
            
            var credentials = new BasicAWSCredentials(accessKeyId, secretKey);

            // Specify the region
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsCloudProvider.Region)
            };
            
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
                using var client = new AmazonS3Client(credentials, config);

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
            var accessKeyId = _secretManager.GetSecret($"{awsCloudProvider.ClientId}-{Shared.Constants.SecretKeys.AwsAccessKeyId}").GetAwaiter().GetResult();
            var secretKey = _secretManager.GetSecret($"{awsCloudProvider.ClientId}-{Shared.Constants.SecretKeys.AwsSecretAccessKey}").GetAwaiter().GetResult();
            
            var credentials = new BasicAWSCredentials(accessKeyId, secretKey);
            
            // Specify the region
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsCloudProvider.Region)
            };
            try
            {
                // Created an S3 client
                using var client = new AmazonS3Client(credentials, config);

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
            var accessKeyId = _secretManager.GetSecret($"{awsCloudProvider.ClientId}-{Shared.Constants.SecretKeys.AwsAccessKeyId}").GetAwaiter().GetResult();
            var secretKey = _secretManager.GetSecret($"{awsCloudProvider.ClientId}-{Shared.Constants.SecretKeys.AwsSecretAccessKey}").GetAwaiter().GetResult();
            
            var credentials = new BasicAWSCredentials(accessKeyId, secretKey);
            
            // Specify the region
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsCloudProvider.Region)
            };
            try
            {
                // Created an S3 client
                using var client = new AmazonS3Client(credentials, config);
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = awsCloudProvider.BucketName,
                    Key = fileName,
                    Expires = DateTime.Now.AddMinutes(expireInMinutes)
                };

                // Get path for request
                return client.GetPreSignedURL(request);
            }
            catch (Exception)
            {
                throw new Exception("File not found");
            }
        }
    }
}