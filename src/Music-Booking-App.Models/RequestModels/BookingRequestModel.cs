namespace Music_Booking_App.Models.RequestModels
{
    public class BookingRequestModel
    {
        public string ArtisteId { get; set; }
        public string EventId { get; set; }
        public decimal ProposedAmount { get; set; }
    }
}
