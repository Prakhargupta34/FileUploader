using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using FileUploader.Shared;
using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FileUploader.AzureCloud;

public class AzureCloud : IAzureCloud
{
    private readonly ISecretManager _secretManager;

    public AzureCloud(ISecretManager secretManager)
    {
        _secretManager = secretManager;
    }

    public async Task UploadFile(IFormFile file, AzureCloudProvider azureCloudProvider)
    {
        try
        {
            var blob = GetBlobClient(azureCloudProvider, file.FileName);
            
            using (Stream stream = file.OpenReadStream())
            {
                blob.Upload(stream);
            }
        }
        catch (Exception)
        {
            throw new Exception($"Unable to upload file");
        }
    }

    public async Task<FileResponse> DownloadFile(string fileName, AzureCloudProvider azureCloudProvider)
    {
        try
        {
            var blob = GetBlobClient(azureCloudProvider, fileName);

            if (await blob.ExistsAsync())
            {
                var res = await blob.DownloadAsync();
                return new FileResponse
                {
                    FileName = fileName,
                    ResponseStream = res.Value.Content,
                    ContentType = res.Value.ContentType
                };
            }
            throw new Exception();
        }
        catch (Exception)
        {
            throw new Exception("File not found");
        }
    }

    public async Task<string> GetShareableUrl(string fileName, AzureCloudProvider azureCloudProvider, int expiryInMins)
    {
        try
        {
            var blobClient = GetBlobClient(azureCloudProvider, fileName);
            
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Resource = "b"
            };
            
            sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryInMins);
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }
        catch (Exception)
        {
            throw new Exception("File not found");
        }
    }

    private BlobClient GetBlobClient(AzureCloudProvider azureCloudProvider, string fileName)
    {
        var connectionString =
            _secretManager.GetSecret($"{azureCloudProvider.ClientId}-{Shared.Constants.SecretKeys.AzureStorageAccountConnectionString}").GetAwaiter().GetResult();
        BlobContainerClient container = new BlobContainerClient(connectionString, azureCloudProvider.StorageContainerName);

        BlobClient blobClient = container.GetBlobClient(fileName);
        return blobClient;
    }
}