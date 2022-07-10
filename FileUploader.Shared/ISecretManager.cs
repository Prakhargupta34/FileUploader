namespace FileUploader.Shared;

public interface ISecretManager
{
    Task<string> GetSecret(string key);
    Task StoreSecret(string key, string value);
}