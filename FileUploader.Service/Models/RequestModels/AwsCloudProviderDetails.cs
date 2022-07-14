using System;

namespace FileUploader.Service.Models.RequestModels;

public class AwsCloudProviderDetails
{
    public string BucketName { get; set; }
    public string Region { get; set; }
    public string SecretAccessKey { get; set; }
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