

namespace Music_Booking_App.Models.RequestModels
{
    public class ResetPasswordRequestModel
    {
        public string Email { get; set; }
        // public string DefaultPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
