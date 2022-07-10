using System;

namespace FileUploader.Service.Models.RequestModels;

public class AzureCloudProvider
{
    public string StorageAccountConnectionString { get; set; }
    public string StorageContainerName { get; set; }

    public Shared.Models.AzureCloudProvider ToModel()
    {
        return new Shared.Models.AzureCloudProvider
        {
            Id = Guid.NewGuid(),
            StorageContainerName = StorageContainerName
        };
    }
}