

using Newtonsoft.Json;

namespace Music_Booking_App.Services.BL.InternalProviders.NotificationMs.DTOs
{
    public class TemplateDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
        [JsonProperty(nameof(Data))]
        public TemplateData Data { get; set; }


        public class TemplateData
        {
            public string Subject { get; set; }
            public string TemplateName { get; set; }
            public string TemplateContent { get; set; }
            public string Id { get; set; }
        }
    }
}
