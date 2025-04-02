namespace Music_Booking_App.Models.ViewModels
{
    public class PaystackResponseViewModel
    {
        public string Messsage { get; set; }
        public bool IsSuccess { get; set; }
        public string Reference { get; set; }
        public string AuthorizationUrl { get; set; }
    }
}
