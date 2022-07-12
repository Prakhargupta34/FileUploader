using System.Linq;
using System.Threading.Tasks;
using FileUploader.Service.Data;
using FileUploader.Service.Interfaces;
using FileUploader.Service.Models;
using FileUploader.Shared.Exceptions;

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
        _dbContext.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public User GetUser(string username, string password)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username == username && user.Password == password);
    }

    public bool IsUserExists(string username)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username == username) != null;
    }
}