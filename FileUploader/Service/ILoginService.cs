using Microsoft.Extensions.Configuration;

namespace FileUploader.Service
{
    public interface ILoginService
    {
        string LoginAndCreateToken(string username, string password, IConfiguration config);
    }
}
