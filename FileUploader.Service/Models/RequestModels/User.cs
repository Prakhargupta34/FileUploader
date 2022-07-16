using System;
using System.ComponentModel.DataAnnotations;

namespace FileUploader.Service.Models.RequestModels
{
    public class User
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        [RegularExpression($"{Shared.Constants.Roles.Administrator}|{Shared.Constants.Roles.User}", ErrorMessage = $"The Role can be either '{Shared.Constants.Roles.Administrator}' or '{Shared.Constants.Roles.User}' only.")]
        public string Role { get; set; }

        public Models.User ToModel(string clientId=null)
        {
            return new Models.User
            {
                Id = Guid.NewGuid(),
                ClientId = Guid.Parse(clientId),
                Username = Username,
                Password = Password,
                EmailAddress = EmailAddress,
                Role = Role
            };
        }
    }
}
