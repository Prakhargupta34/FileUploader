using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileUploader.Service.Data;
using FileUploader.Service.Interfaces;
using FileUploader.Service.Models;
using FileUploader.Service.Models.RequestModels;
using FileUploader.Service.Models.ResponseModels;
using FileUploader.Shared;
using FileUploader.Shared.Constants;
using FileUploader.Shared.Exceptions;
using User = FileUploader.Service.Models.RequestModels.User;

namespace FileUploader.Service.Services;

public class ClientService: IClientService
{
    private readonly FileUploaderDbContext _context;
    private readonly ISecretManager _secretManager;
    private readonly IUserService _userService;

    public ClientService(FileUploaderDbContext context, ISecretManager secretManager, IUserService userService)
    {
        _context = context;
        _secretManager = secretManager;
        _userService = userService;
    }
    public async Task<ClientResponse> CreateClient(ClientRequest clientRequest)
    {
        if (_userService.IsUserExists(clientRequest.AdminUser.Username))
            throw new BadRequestException("Admin username provided is already taken");
        
        clientRequest.CloudProviderType = clientRequest.CloudProviderType.Trim().ToLower();
        var cloudProviderType = clientRequest.CloudProviderType;
        
        var clientId = Guid.NewGuid();
        Guid cloudProviderId =  CreateCloudProvider(clientRequest, clientId);
        var client = new Client()
        {
            Id = clientId,
            Name = clientRequest.Name,
            CloudProviderId = cloudProviderId,
            CloudProviderType = cloudProviderType,
        };
        
        _context.Add(client);
        await _context.SaveChangesAsync();
        
        CreateAdminUser(clientRequest.AdminUser, clientId);
        
        return new ClientResponse()
        {
            ClientId = client.Id
        };
    }
    
    public Guid CreateCloudProvider(ClientRequest clientRequest, Guid clientId)
    {
        var cloudProviderType = clientRequest.CloudProviderType;
        Guid cloudProviderId;
        if (cloudProviderType == CloudProviderType.AWS)
        {
            var awsCloudProvider = clientRequest.AwsCloudProvider.ToModel();
            awsCloudProvider.ClientId = clientId;
                _context.Add(awsCloudProvider);
            cloudProviderId = awsCloudProvider.Id;
            
            _secretManager.StoreSecret($"{clientId}-{Shared.Constants.SecretKeys.AwsAccessKeyId}",
                clientRequest.AwsCloudProvider.AccessKeyId).GetAwaiter().GetResult();
            _secretManager.StoreSecret($"{clientId}-{Shared.Constants.SecretKeys.AwsSecretAccessKey}",
                clientRequest.AwsCloudProvider.SecretAccessKey).GetAwaiter().GetResult();
        }
        else if (cloudProviderType == CloudProviderType.Azure)
        {
            var azureCloudProvider = clientRequest.AzureCloudProvider.ToModel();
            azureCloudProvider.ClientId = clientId;
            _context.Add(azureCloudProvider);
            cloudProviderId = azureCloudProvider.Id;
            
            _secretManager.StoreSecret($"{clientId}-{Shared.Constants.SecretKeys.AzureStorageAccountConnectionString}",
                clientRequest.AzureCloudProvider.StorageAccountConnectionString).GetAwaiter().GetResult();
        }
        else
        {
            throw new Exception("Cloud provider not supported");
        }

        return cloudProviderId;
    }

    private void CreateAdminUser(User user, Guid clientId)
    {
        var adminUser = user.ToModel(clientId.ToString());
        adminUser.Role = Roles.Administrator;
        _userService.CreateUser(adminUser);
    }
}