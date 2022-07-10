using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace FileUploader.Shared;

public class AzureSecretManger : ISecretManager
{
    private readonly IConfiguration _configuration;

    public AzureSecretManger(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<string> GetSecret(string key)
    {
        string kvUri = _configuration[Constants.SecretKeys.Azure.KeyVaultUrl];
        
        var credentials = new ClientSecretCredential(_configuration[Constants.SecretKeys.Azure.TenantId],
            _configuration[Constants.SecretKeys.Azure.ClientId], _configuration[Constants.SecretKeys.Azure.Secret]);
        
        var client = new SecretClient(new Uri(kvUri), credentials);

        return client.GetSecretAsync(key).GetAwaiter().GetResult().Value.Value;
    }

    public async Task StoreSecret(string key, string value)
    {
        string kvUri = _configuration[Constants.SecretKeys.Azure.KeyVaultUrl];

        var credentials = new ClientSecretCredential(_configuration[Constants.SecretKeys.Azure.TenantId],
            _configuration[Constants.SecretKeys.Azure.ClientId], _configuration[Constants.SecretKeys.Azure.Secret]);
        
        var client = new SecretClient(new Uri(kvUri), credentials);

        client.SetSecretAsync(key, value).GetAwaiter().GetResult();
    }
}