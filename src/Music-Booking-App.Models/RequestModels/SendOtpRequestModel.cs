using Music_Booking_App.Models.Enums;

namespace Music_Booking_App.Models.RequestModels
{
    public class SendOtpRequestModel
    {
        public string Email { get; set; }
        public EmailTemplates AuthType { get; set; }
    }
}
