using System.Threading.Tasks;
using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FileUploader.AzureCloud
{
    public interface IAzureCloud
    {
        Task UploadFile(IFormFile file, AzureCloudProvider azureCloudProvider);
        Task<FileResponse> DownloadFile(string fileName, AzureCloudProvider azureCloudProvider);
        Task<string> GetShareableUrl(string fileName, AzureCloudProvider azureCloudProvider, int expiryInMins);
    }
}