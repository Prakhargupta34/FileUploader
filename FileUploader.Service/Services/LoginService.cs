using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FileUploader.Service.Exceptions;
using FileUploader.Service.Interfaces;
using FileUploader.Service.Models;
using FileUploader.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FileUploader.Service.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserService _userService;
        public LoginService(IUserService userService)
        {
            _userService = userService;
        }

        public string LoginAndCreateToken(string username, string password, IConfiguration config)
        {
            var user = Authenticate(username, password);

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
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimType.ClientId, user.ClientId.ToString())
            };

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
              config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(string username, string password)
        {
            User user = _userService.GetUser(username, password);
            if (user == null)
            {
                throw new UserNotFoundException("Either username or password doesn't match");
            }
            return user;
        }
    }
}
