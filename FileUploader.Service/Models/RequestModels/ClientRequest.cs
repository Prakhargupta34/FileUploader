namespace FileUploader.Service.Models.RequestModels;

public class ClientRequest
{
    public string Name { get; set; }
    
    public string CloudProviderType { get; set; }

    public AwsCloudProviderDetails AwsCloudProviderDetails { get; set; }
    public AzureCloudProviderDetails AzureCloudProviderDetails { get; set; }
    
    public User AdminUser { get; set; }
}