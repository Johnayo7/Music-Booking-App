

namespace Music_Booking_App.Models.DTOs
{
    public class ValidationResultDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }

        public static ValidationResultDto Success()
        {
            return new ValidationResultDto
            {
                IsValid = true,
                Message = "Validation successful."
            };
        }

        public static ValidationResultDto Failure(string message)
        {
            return new ValidationResultDto
            {
                IsValid = false,
                Message = message
            };
        }
    }
}
