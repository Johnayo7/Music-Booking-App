namespace Music_Booking_App.Services.BL.ExternalProviders.Paystack.DTOs
{
    public class PaymentRequestDto
    {
        public string Email { get; set; }
        public decimal Amount { get; set; }
    }
}
