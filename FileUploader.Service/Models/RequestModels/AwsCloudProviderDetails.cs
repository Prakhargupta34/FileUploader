using System;
using System.ComponentModel.DataAnnotations;

namespace FileUploader.Service.Models.RequestModels;

public class AwsCloudProviderDetails
{
    [Required]
    public string BucketName { get; set; }
    [Required]
    public string Region { get; set; }
    [Required]
    public string SecretAccessKey { get; set; }
    [Required]
    public string AccessKeyId { get; set; }

    public Shared.Models.AwsCloudProviderDetails ToModel()
    {
        return new Shared.Models.AwsCloudProviderDetails
        {
            Id = Guid.NewGuid(),
            BucketName = BucketName,
            Region = Region
        };
    }
}