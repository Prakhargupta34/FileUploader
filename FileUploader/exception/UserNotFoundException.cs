using System;

namespace FileUploader.exception
{
    
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message)
            : base(message)
        {
        }
    }
}
