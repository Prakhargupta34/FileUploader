using System;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileUploader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                    var root = config.Build();
                    AddAzureKeyvaultConfigurationSource(root, config);
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        
        private static void AddAzureKeyvaultConfigurationSource(IConfigurationRoot root, IConfigurationBuilder config){
            var tenantId = root[Shared.Constants.SecretKeys.Azure.TenantId];
            var clientId = root[Shared.Constants.SecretKeys.Azure.ClientId];
            var secret = root[Shared.Constants.SecretKeys.Azure.Secret];
            var url = root[Shared.Constants.SecretKeys.Azure.KeyVaultUrl];
                    
            var credential = new ClientSecretCredential(tenantId, clientId, secret);
            config.AddAzureKeyVault(new Uri(url), credential);
        }
    }
}