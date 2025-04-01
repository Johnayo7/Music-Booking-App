namespace Music_Booking_App.Models.RequestModels
{
    public class UpdateEventRequestModel : CreateEventRequestModel
    {
        public Guid Id { get; set; }
    }
}
