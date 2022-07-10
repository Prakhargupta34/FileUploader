using System;

namespace FileUploader.Service.Models.RequestModels
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
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
