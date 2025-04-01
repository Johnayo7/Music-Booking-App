namespace Music_Booking_App.Models.RequestModels
{
    public class CreateEventRequestModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public decimal TicketPrice { get; set; }
    }
}
