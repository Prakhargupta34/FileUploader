using System;

namespace FileUploader.Service.Models.RequestModels;

public class AzureCloudProviderDetails
{
    public string StorageAccountConnectionString { get; set; }
    public string StorageContainerName { get; set; }

    public Shared.Models.AzureCloudProviderDetails ToModel()
    {
        return new Shared.Models.AzureCloudProviderDetails
        {
            Id = Guid.NewGuid(),
            StorageContainerName = StorageContainerName
        };
    }
}