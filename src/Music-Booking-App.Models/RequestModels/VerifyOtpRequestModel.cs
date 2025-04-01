namespace Music_Booking_App.Models.RequestModels
{
    public class VerifyOtpRequestModel
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
