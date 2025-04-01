namespace Music_Booking_App.Models.RequestModels
{
    public class CreateArtisteRequestModel
    {
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Bio { get; set; }
        public decimal BookingRate { get; set; }
    }
}
