using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FileUploader.Controllers
{
    [Route("[controller]")]
    public class Files : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public Files(IConfiguration _configuration)
        {
            this._configuration = _configuration;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            await using var memoryStr = new MemoryStream();
            await file.CopyToAsync(memoryStr);

            //var fileExt = Path.GetExtension(file.Name);
            var objName = $"{file.FileName}";
            
            var credentials = new BasicAWSCredentials(_configuration["AWS_ACCESS_KEY_ID"].Trim(), _configuration["AWS_SECRET_ACCESS_KEY"].Trim());

            // Specify the region
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.APSouth1
            };
            
            try
            {
                // Create the upload request
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = memoryStr,
                    Key = objName,
                    BucketName = "testfiles123",
                    CannedACL = S3CannedACL.NoACL
                };

                // Created an S3 client
                using var client = new AmazonS3Client(credentials, config);

                // upload utility to s3
                var transferUtiltiy = new TransferUtility(client);

                // We are actually uploading the file to S3
                await transferUtiltiy.UploadAsync(uploadRequest);
                
                return Ok($"{objName} has been uploaded successfully");
            }
            catch(AmazonS3Exception ex)
            {
                return BadRequest($"{ex.StatusCode}-{ex.Message}");
            }
            catch(Exception ex)
            {
                return BadRequest($"500-{ex.Message}");
            }
        }
        
        [HttpGet("getfile")]
        public async Task<IActionResult> GetFile([FromQuery]string fileName)
        {
            var credentials = new BasicAWSCredentials(_configuration["AWS_ACCESS_KEY_ID"].Trim(), _configuration["AWS_SECRET_ACCESS_KEY"].Trim());

            // Specify the region
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.APSouth1
            };
            
            try
            {
                // Created an S3 client
                using var client = new AmazonS3Client(credentials, config);

                var res = await client.GetObjectAsync(new GetObjectRequest()
                {
                    Key = fileName,
                    BucketName = "testfiles123"
                });

                return File(res.ResponseStream, res.Headers.ContentType);
            }
            catch(AmazonS3Exception ex)
            {
                return BadRequest($"{ex.StatusCode}-{ex.Message}");
            }
            catch(Exception ex)
            {
                return BadRequest($"500-{ex.Message}");
            }
        }
    }
}