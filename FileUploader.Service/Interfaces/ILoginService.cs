using Microsoft.Extensions.Configuration;

namespace FileUploader.Service.Interfaces
{
    public interface ILoginService
    {
        string LoginAndCreateToken(string username, string password, IConfiguration config);
    }
}
