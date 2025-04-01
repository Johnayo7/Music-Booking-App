
namespace Music_Booking_App.Models.DTOs
{
    public class ApplicationUserDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string AllowedIp { get; set; }
        public string IsActive { get; set; }
    }
}
