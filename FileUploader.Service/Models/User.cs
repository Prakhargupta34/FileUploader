using System;

namespace FileUploader.Service.Models;

public class User
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string EmailAddress { get; set; }
    public string Role { get; set; }
}