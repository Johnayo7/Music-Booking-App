namespace Music_Booking_App.Models.ViewModels
{
    public class EventDetailsViewModel : EventsViewModel
    {
        public DateTime Date { get; set; }
        public decimal TicketPrice { get; set; }

    }
}
