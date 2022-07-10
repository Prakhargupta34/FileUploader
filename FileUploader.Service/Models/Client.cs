using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FileUploader.Service.Models;

public class Client
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public List<User> Users { get; set; }
    public string CloudProviderType { get; set; }

    public Guid CloudProviderId { get; set; }
}