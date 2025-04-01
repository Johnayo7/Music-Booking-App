

using Newtonsoft.Json;

namespace Music_Booking_App.Services.BL.InternalProviders.NotificationMs.DTOs
{
    public class AddTemplateDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
        [JsonProperty(nameof(Data))]
        public AddTemplateData Data { get; set; }

        public class AddTemplateData
        {
            public string Id { get; set; }
        }
    }
}
