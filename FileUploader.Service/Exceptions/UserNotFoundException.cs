using System;

namespace FileUploader.Service.Exceptions
{
    
    public class UserNotFoundException : BadRequestException
    {
        public UserNotFoundException(string message)
            : base(message)
        {
        }
    }
}
