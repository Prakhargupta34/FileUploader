namespace FileUploader.Service.Models.RequestModels;

public class ClientRequest
{
    public string Name { get; set; }
    
    public string CloudProviderType { get; set; }

    public AwsCloudProvider AwsCloudProvider { get; set; }
    public AzureCloudProvider AzureCloudProvider { get; set; }
    
    public User AdminUser { get; set; }
}