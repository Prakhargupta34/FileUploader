using FileUploader.Service.Models;
using FileUploader.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FileUploader.Service.Data;

public class FileUploaderDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    
    public DbSet<AwsCloudProvider> AwsCloudProviders { get; set; }
    
    public DbSet<AzureCloudProvider> AzureCloudProviders { get; set; }
    
    public DbSet<User> Users { get; set; }

    public FileUploaderDbContext(DbContextOptions<FileUploaderDbContext> options) : base(options)
    {
    }
}