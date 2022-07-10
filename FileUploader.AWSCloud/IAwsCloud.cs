using System.Threading.Tasks;
using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FileUploader.AWSCloud
{
    public interface IAwsCloud
    {
        Task UploadFile(IFormFile file, AwsCloudProvider awsCloudProvider);
        Task<FileResponse> DownloadFile(string fileName, AwsCloudProvider awsCloudProvider);

        Task<string> GetShareableUrl(string fileName, AwsCloudProvider awsCloudProvider, int expireInMinutes);
    }
}