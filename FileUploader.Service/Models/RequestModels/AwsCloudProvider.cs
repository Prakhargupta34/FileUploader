using System;

namespace FileUploader.Service.Models.RequestModels;

public class AwsCloudProvider
{
    public string BucketName { get; set; }
    public string Region { get; set; }
    public string SecretAccessKey { get; set; }
    public string AccessKeyId { get; set; }

    public Shared.Models.AwsCloudProvider ToModel()
    {
        return new Shared.Models.AwsCloudProvider
        {
            Id = Guid.NewGuid(),
            BucketName = BucketName,
            Region = Region,
            SecretAccessKey = SecretAccessKey,
            AccessKeyId = AccessKeyId,
        };
    }
}