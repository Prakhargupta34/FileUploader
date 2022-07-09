using FileUploader.Models;

namespace FileUploader.Database
{
    public interface IDatabase
    {
        User getUser(string username, string password);
    }
}
