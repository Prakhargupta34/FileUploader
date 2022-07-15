namespace FileUploader.Shared.Models;

public class AwsCloudProviderDetails : CloudProviderDetails
{
    public Guid Id { get; set; }
    
    public Guid ClientId { get; set; }
    public string BucketName { get; set; }
    public string Region { get; set; }
}