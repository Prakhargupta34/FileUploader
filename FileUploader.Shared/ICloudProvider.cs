using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FileUploader.Shared;

public interface ICloudProvider
{
    Task UploadFile(IFormFile file, CloudProviderDetails cloudProviderDetails);
    Task<FileResponse> DownloadFile(string fileName, CloudProviderDetails cloudProviderDetails);
    Task<string> GetShareableUrl(string fileName, CloudProviderDetails cloudProviderDetails, int expireInMinutes);
}