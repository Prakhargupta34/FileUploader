namespace FileUploader.Shared.Models;

public class AzureCloudProviderDetails : CloudProviderDetails
{
    public Guid Id { get; set; }
    
    public Guid ClientId { get; set; }
    public string StorageContainerName { get; set; }
}