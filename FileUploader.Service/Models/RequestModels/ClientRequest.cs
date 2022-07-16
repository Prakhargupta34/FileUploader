using System.ComponentModel.DataAnnotations;

namespace FileUploader.Service.Models.RequestModels;

public class ClientRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string CloudProviderType { get; set; }
    public AwsCloudProviderDetails AwsCloudProviderDetails { get; set; }
    public AzureCloudProviderDetails AzureCloudProviderDetails { get; set; }
    
    [Required]
    public User AdminUser { get; set; }
}