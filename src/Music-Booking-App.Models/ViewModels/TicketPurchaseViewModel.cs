namespace Music_Booking_App.Models.ViewModels
{
    public class TicketPurchaseViewModel
    {
        public Guid Id { get; set; }
        public string EventName { get; set; }
        public string BuyerName { get; set; }
        public decimal AmountPaid { get; set; }
        public int NoOfTickets { get; set; }
        public string ReferenceId { get; set; }
        public string PaymentStatus { get; set; }
        public string FeeType { get; set; }
        public string ReferenceUrl { get; set; }

        public string OrganizerName { get; set; }
    }
}
