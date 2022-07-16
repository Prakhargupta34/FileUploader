using System;
using System.ComponentModel.DataAnnotations;

namespace FileUploader.Service.Models.RequestModels;

public class AzureCloudProviderDetails
{
    [Required]
    public string StorageAccountConnectionString { get; set; }
    [Required]
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