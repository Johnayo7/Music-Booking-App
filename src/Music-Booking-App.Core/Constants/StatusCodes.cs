

namespace Music_Booking_App.Core.Constants
{
    public class StatusCodes
    {
        // General status codes
        public const string Successful = "00";
        public const string SqlException = "07";
        public const string AuditLogError = "05";
        public const string GeneralError = "06";
        public const string ModelValidationError = "09";
        public const string FatalError = "96";
        public const string NoRecordFound = "25";
        public const string DuplicateRecord = "26";
        public const string NubanGenerationFailed = "29";
        public const string InsufficientFunds = "51";
        public const string TimeoutOrExpired = "08";
        public const string Locked = "423";
        public const string UnAuthorized = "401";
        public const string BadRequest = "400";
        public const string Invalid = "19";
        public const string Forbidden = "403";
        public const string TooManyRequests = "429";
    }
}
