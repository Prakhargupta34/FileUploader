namespace FileUploader.Shared;

public interface ICloudProviderFactory
{
    ICloudProvider GetCloudProvider(string cloudProviderType);
}