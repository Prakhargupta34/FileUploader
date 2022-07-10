using System.Threading.Tasks;
using FileUploader.Service.Models;
using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FileUploader.Service.Interfaces;

public interface IFileService
{
    Task UploadFile(IFormFile file, string clientId);
    Task<FileResponse> DownloadFile(string fileName, string clientId);
    
    Task<string> GetShareableUrl(string fileName, int expiryInMins, string clientId);
}