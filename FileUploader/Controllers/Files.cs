using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3;
using Amazon.S3.Model;
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
        public async Task<IActionResult> Upload()
        {
            var credentials = new BasicAWSCredentials(_configuration["AWS_ACCESS_KEY_ID"].Trim(), _configuration["AWS_SECRET_ACCESS_KEY"].Trim());
            var s3Client = new AmazonS3Client(credentials, RegionEndpoint.APSouth1);
            var bucket = await s3Client.ListBucketsAsync();
            return Ok();
        }
    }
}