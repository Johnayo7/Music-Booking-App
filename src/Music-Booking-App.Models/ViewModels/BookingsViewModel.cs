namespace Music_Booking_App.Models.ViewModels
{
    public class BookingsViewModel
    {
        public Guid Id { get; set; }
        public string ArtisteName { get; set; }
        public string EventName { get; set; }
        public string OrganizerName { get; set; }
        public decimal ProposedAmount { get; set; }
        public string Status { get; set; }
    }
}
