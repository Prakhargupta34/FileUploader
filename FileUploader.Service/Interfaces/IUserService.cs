using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileUploader.Service.Models.RequestModels;

namespace FileUploader.Service.Interfaces;

public interface IUserService
{
    Task CreateUser(Models.User user);
    Models.User GetUser(string username, string password);

    bool IsUserExists(string username);
}