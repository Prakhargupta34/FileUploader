using System.Net;

namespace FileUploader.Shared.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
        Data.Add(Shared.Constants.General.StatusCode, (int) HttpStatusCode.BadRequest);
    }
}