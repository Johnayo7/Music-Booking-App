namespace Music_Booking_App.Models.Entiites
{
    public class Payment : BaseEntity
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
