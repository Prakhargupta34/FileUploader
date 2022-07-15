using System;
using System.Linq;
using System.Threading.Tasks;
using FileUploader.Service.Data;
using FileUploader.Service.Interfaces;
using FileUploader.Service.Models;
using FileUploader.Shared;
using FileUploader.Shared.Constants;
using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FileUploader.Service.Services;

public class FileService : IFileService
{
    private readonly FileUploaderDbContext _context;
    private readonly ICloudProviderFactory _cloudProviderFactory;

    public FileService(FileUploaderDbContext context, ICloudProviderFactory cloudProviderFactory)
    {
        _context = context;
        _cloudProviderFactory = cloudProviderFactory;
    }
    public async Task UploadFile(IFormFile file, string clientId)
    {
        var client = _context.Clients.FirstOrDefault(client => client.Id.ToString() == clientId);
        var cloudProviderDetails = GetCloudProviderDetails(client);
        var cloudProvider = _cloudProviderFactory.GetCloudProvider(client.CloudProviderType);
        cloudProvider.UploadFile(file, cloudProviderDetails).GetAwaiter().GetResult();
    }

    public async Task<FileResponse> DownloadFile(string fileName, string clientId)
    {
        var client = _context.Clients.FirstOrDefault(client => client.Id.ToString() == clientId);
        var cloudProviderDetails = GetCloudProviderDetails(client);
        var cloudProvider = _cloudProviderFactory.GetCloudProvider(client.CloudProviderType);
        return cloudProvider.DownloadFile(fileName, cloudProviderDetails).GetAwaiter().GetResult();
    }

    public async Task<string> GetShareableUrl(string fileName, int expiryInMins, string clientId)
    {
        var client = _context.Clients.FirstOrDefault(client => client.Id.ToString() == clientId);
        var cloudProviderDetails = GetCloudProviderDetails(client);
        var cloudProvider = _cloudProviderFactory.GetCloudProvider(client.CloudProviderType);
        return cloudProvider.GetShareableUrl(fileName, cloudProviderDetails, expiryInMins).GetAwaiter().GetResult();
    }

    private CloudProviderDetails GetCloudProviderDetails(Client client)
    {
        if (client?.CloudProviderType == CloudProviderType.AWS)
        {
            return _context.AwsCloudProviders.FirstOrDefault(provider => provider.Id == client.CloudProviderId);
        }
        if (client?.CloudProviderType == CloudProviderType.Azure)
        {
            return _context.AzureCloudProviders.FirstOrDefault(provider => provider.Id == client.CloudProviderId);
        }
        throw new Exception("Cloud provider not supported");
    }
}