namespace Music_Booking_App.Models.RequestModels
{
    public class TicketPurchaseRequestModel
    {
        public Guid EventId { get; set; }
        public int NoOfTickets { get; set; }
    }
}
