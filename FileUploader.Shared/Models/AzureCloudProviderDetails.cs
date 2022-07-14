namespace FileUploader.Shared.Models;

public class AzureCloudProviderDetails
{
    public Guid Id { get; set; }
    
    public Guid ClientId { get; set; }
    public string StorageContainerName { get; set; }
}