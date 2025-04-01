

namespace Music_Booking_App.Core.Helpers
{
    public class ApiResponse
    {
        public string StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public string Result { get; set; }
    }
}
