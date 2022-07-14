using System;
using FileUploader.Shared;

namespace FileUploader.Service;

public class CloudProviderFactory : ICloudProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CloudProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public ICloudProvider GetCloudProvider(string cloudProviderType)
    {
        switch (cloudProviderType)
        {
            case Shared.Constants.CloudProviderType.AWS : return _serviceProvider.GetService(typeof(AwsCloudProvider.AwsCloudProvider)) as AwsCloudProvider.AwsCloudProvider;
            case Shared.Constants.CloudProviderType.Azure : return _serviceProvider.GetService(typeof(AzureCloudProvider.AzureCloudProvider)) as AzureCloudProvider.AzureCloudProvider;
            default: throw new Exception("Cloud provider not supported");
        }
    }
}