namespace Music_Booking_App.Models.RequestModels
{
    public class UpdateBookingRequestModel : BookingRequestModel
    {
        public Guid BookingId { get; set; }
    }
}
