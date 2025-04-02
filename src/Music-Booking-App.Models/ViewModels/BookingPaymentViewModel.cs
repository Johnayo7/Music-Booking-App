namespace Music_Booking_App.Models.ViewModels
{
    public class BookingPaymentViewModel
    {
        public Guid Id { get; set; }
        public string EventName { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceId { get; set; }
        public string PaymentStatus { get; set; }
        public string FeeType { get; set; }
        public string ReferenceUrl { get; set; }
        public string OrganizerName { get; set; }
        public string ArtisteName { get; set; }
    }
}
