using Music_Booking_App.Models.Enums;

namespace Music_Booking_App.Models.RequestModels
{
    public class SignUpRequestModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public UserCategory UserCategory { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
