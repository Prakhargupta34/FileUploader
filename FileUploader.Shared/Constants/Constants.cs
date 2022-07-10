namespace FileUploader.Shared.Constants;

public static class CloudProviderType
{
    public const string AWS = "aws";
    public const string Azure = "azure";
}

public static class ClaimType
{
    public const string ClientId = "clientId";
}

public static class General
{
    public const string StatusCode = "StatusCode";
}

public static class SecretKeys
{
    public const string AwsAccessKeyId = "AwsAccessKeyId";
    public const string AwsSecretAccessKey = "AwsSecretAccessKey";
    public const string AzureStorageAccountConnectionString = "AzureStorageAccountConnectionString";

    public static class Azure
    {
        public const string KeyVaultUrl = "AzureKeyVault:Url";
        public const string TenantId = "AzureKeyVault:TenantId";
        public const string ClientId = "AzureKeyVault:ClientId";
        public const string Secret = "AzureKeyVault:Secret";
    }

}

public static class Roles
{
    public const string Administrator = "Administrator";
    public const string User = "User";
}
