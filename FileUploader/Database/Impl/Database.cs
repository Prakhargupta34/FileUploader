using FileUploader.Models;
using System.Collections.Generic;
using System.Linq;

namespace FileUploader.Database.Impl
{
    public class Database : IDatabase
    {
        private static List<User> Users = new List<User>()
        {
            new User() { Username = "jason_admin", EmailAddress = "jason.admin@email.com", Password = "MyPass_w0rd", Role = "Administrator" },
            new User() { Username = "elyse_seller", EmailAddress = "elyse.seller@email.com", Password = "MyPass_w0rd", Role = "Seller" },
        };

        public User getUser(string username, string password)
        {
            return Users.FirstOrDefault(o => o.Username.ToLower() == username.ToLower() && o.Password == password);
        }
    }
}
