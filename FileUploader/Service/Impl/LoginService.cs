using FileUploader.Database;
using FileUploader.exception;
using FileUploader.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;

namespace FileUploader.Service.Impl
{
    public class LoginService : ILoginService
    {
        private IDatabase db;
        public LoginService(IDatabase db)
        {
            this.db = db;
        }

        public string LoginAndCreateToken(string username, string password, IConfiguration config)
        {
            User user = authenticate(username, password);

            return GenerateToken(user, config);
        }

        private string GenerateToken(User user, IConfiguration config)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
              config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User authenticate(string username, string password)
        {
            User user = db.getUser(username, password);
            if (user == null)
            {
                throw new UserNotFoundException("Either username or password does n't match");
            }
            return user;
        }
    }
}
