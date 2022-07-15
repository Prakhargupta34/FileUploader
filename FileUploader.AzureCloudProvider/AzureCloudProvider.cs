using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using FileUploader.Shared;
using FileUploader.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace FileUploader.AzureCloudProvider;

public class AzureCloudProvider : ICloudProvider
{
    private readonly ISecretManager _secretManager;

    public AzureCloudProvider(ISecretManager secretManager)
    {
        _secretManager = secretManager;
    }

    public async Task UploadFile(IFormFile file, CloudProviderDetails azureCloudDetails)
    {
        var azureCloudProviderDetails = azureCloudDetails as AzureCloudProviderDetails;
        try
        {
            var blob = GetBlobClient(azureCloudProviderDetails, file.FileName);
            
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

    public async Task<FileResponse> DownloadFile(string fileName, CloudProviderDetails azureCloudDetails)
    {
        var azureCloudProviderDetails = azureCloudDetails as AzureCloudProviderDetails;
        try
        {
            var blob = GetBlobClient(azureCloudProviderDetails, fileName);

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

    public async Task<string> GetShareableUrl(string fileName, CloudProviderDetails azureCloudDetails, int expiryInMins)
    {        
        var azureCloudProviderDetails = azureCloudDetails as AzureCloudProviderDetails;
        try
        {
            var blobClient = GetBlobClient(azureCloudProviderDetails, fileName);
            if (!await blobClient.ExistsAsync())
                throw new Exception();
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

    private BlobClient GetBlobClient(AzureCloudProviderDetails azureCloudProviderDetails, string fileName)
    {
        var connectionString =
            _secretManager.GetSecret($"{azureCloudProviderDetails.ClientId}-{Shared.Constants.SecretKeys.AzureStorageAccountConnectionString}").GetAwaiter().GetResult();
        BlobContainerClient container = new BlobContainerClient(connectionString, azureCloudProviderDetails.StorageContainerName);

        BlobClient blobClient = container.GetBlobClient(fileName);
        return blobClient;
    }
}