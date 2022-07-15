using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FileUploader.Service.Data;
using FileUploader.Service.Interfaces;
using FileUploader.Service.Models;
using FileUploader.Shared.Exceptions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace FileUploader.Service.Services;

public class UserService : IUserService
{
    private readonly FileUploaderDbContext _dbContext;

    public UserService(FileUploaderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateUser(User user)
    {
        if (IsUserExists(user.Username))
            throw new BadRequestException("Username provided is already taken");
        
        var salt = GetPasswordSalt();
        var passwordHash = GetPasswordHash(salt, user.Password);
        user.Password = passwordHash;
        user.Salt = Convert.ToBase64String(salt);
        
        _dbContext.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public User GetUser(string username, string password)
    {
        var user = _dbContext.Users.FirstOrDefault(user => user.Username == username);
        if (user == null)
            return null;
        var passwordHash = GetPasswordHash(Convert.FromBase64String(user.Salt), password);
        if (passwordHash == user.Password)
            return user;
        return null;
    }

    public bool IsUserExists(string username)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username == username) != null;
    }

    private byte[] GetPasswordSalt()
    {
        byte[] salt = new byte[128 / 8];
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            rngCsp.GetNonZeroBytes(salt);
        }
        return salt;
    }

    private string GetPasswordHash(byte[] salt, string password)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
    }
}