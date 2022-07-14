using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FileUploader.Shared;

public interface ICloudProvider
{
    Task UploadFile(IFormFile file, object cloudProviderDetails);
    Task<FileResponse> DownloadFile(string fileName, object cloudProviderDetails);
    Task<string> GetShareableUrl(string fileName, object cloudProviderDetails, int expireInMinutes);
}