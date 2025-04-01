

namespace Music_Booking_App.Core.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base("Authentication Failed.")
        {
        }

        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException() : base("You are not authorized to access this resource.")
        {
        }

        public ForbiddenException(string message) : base(message)
        {
        }

        public ForbiddenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
