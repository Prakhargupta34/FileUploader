using FileUploader.Service.Models;
using FileUploader.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FileUploader.Service.Data;

public class FileUploaderDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    
    public DbSet<AwsCloudProviderDetails> AwsCloudProviders { get; set; }
    
    public DbSet<AzureCloudProviderDetails> AzureCloudProviders { get; set; }
    
    public DbSet<User> Users { get; set; }

    public FileUploaderDbContext(DbContextOptions<FileUploaderDbContext> options) : base(options)
    {
    }
}