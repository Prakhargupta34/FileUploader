namespace FileUploader.Shared.Models;

public class AzureCloudProvider
{
    public Guid Id { get; set; }
    
    public Guid ClientId { get; set; }
    public string StorageContainerName { get; set; }
}