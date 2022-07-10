using System;
using System.Linq;
using System.Threading.Tasks;
using FileUploader.AWSCloud;
using FileUploader.AzureCloud;
using FileUploader.Service.Data;
using FileUploader.Service.Interfaces;
using FileUploader.Service.Models;
using FileUploader.Shared.Constants;
using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FileUploader.Service.Services;

public class FileService : IFileService
{
    private readonly FileUploaderDbContext _context;
    private readonly IAwsCloud _awsCloud;
    private readonly IAzureCloud _azureCloud;

    public FileService(FileUploaderDbContext context, IAwsCloud awsCloud, IAzureCloud azureCloud)
    {
        _context = context;
        _awsCloud = awsCloud;
        _azureCloud = azureCloud;
    }
    public async Task UploadFile(IFormFile file, string clientId)
    {
        var client = _context.Clients.FirstOrDefault(client => client.Id.ToString() == clientId);
        if (client.CloudProviderType == CloudProviderType.AWS)
        {
            var awsCloudProvider =
                _context.AwsCloudProviders.FirstOrDefault(provider => provider.Id == client.CloudProviderId);
            _awsCloud.UploadFile(file, awsCloudProvider).GetAwaiter().GetResult();
        }
        else if (client.CloudProviderType == CloudProviderType.Azure)
        {
            var azureCloudProvider =
                _context.AzureCloudProviders.FirstOrDefault(provider => provider.Id == client.CloudProviderId);
            _azureCloud.UploadFile(file, azureCloudProvider).GetAwaiter().GetResult();
        }
        else
        {
            throw new Exception($"Unable to upload file");
        }
    }

    public async Task<FileResponse> DownloadFile(string fileName, string clientId)
    {
        var client = _context.Clients.FirstOrDefault(client => client.Id.ToString() == clientId);
        if (client.CloudProviderType == CloudProviderType.AWS)
        {
            var awsCloudProvider =
                _context.AwsCloudProviders.FirstOrDefault(provider => provider.Id == client.CloudProviderId);
            return _awsCloud.DownloadFile(fileName, awsCloudProvider).GetAwaiter().GetResult();
        }
        if (client.CloudProviderType == CloudProviderType.Azure)
        {
            var azureCloudProvider =
                _context.AzureCloudProviders.FirstOrDefault(provider => provider.Id == client.CloudProviderId);
            return _azureCloud.DownloadFile(fileName, azureCloudProvider).GetAwaiter().GetResult();
        }
        else
        {
            throw new Exception("Unable to retrieve file");
        }
    }

    public async Task<string> GetShareableUrl(string fileName, int expiryInMins, string clientId)
    {
        var client = _context.Clients.FirstOrDefault(client => client.Id.ToString() == clientId);
        if (client.CloudProviderType == CloudProviderType.AWS)
        {
            var awsCloudProvider =
                _context.AwsCloudProviders.FirstOrDefault(provider => provider.Id == client.CloudProviderId);
            return _awsCloud.GetShareableUrl(fileName, awsCloudProvider, expiryInMins).GetAwaiter().GetResult();
        }
        if (client.CloudProviderType == CloudProviderType.Azure)
        {
            var azureCloudProvider =
                _context.AzureCloudProviders.FirstOrDefault(provider => provider.Id == client.CloudProviderId);
            return _azureCloud.GetShareableUrl(fileName, azureCloudProvider, expiryInMins).GetAwaiter().GetResult();
        }
        else
        {
            throw new Exception("Unable to retrieve file");
        }
    }
}