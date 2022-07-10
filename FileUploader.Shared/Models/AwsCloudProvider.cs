namespace FileUploader.Shared.Models;

public class AwsCloudProvider
{
    public Guid Id { get; set; }
    
    public Guid ClientId { get; set; }
    public string BucketName { get; set; }
    public string Region { get; set; }
    public string SecretAccessKey { get; set; }
    public string AccessKeyId { get; set; }
}