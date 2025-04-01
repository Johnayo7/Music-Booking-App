

using Newtonsoft.Json;

namespace Music_Booking_App.Services.BL.InternalProviders.NotificationMs.DTOs
{
    public class TemplateTypeDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
        [JsonProperty(nameof(Data))]
        public TemplateTypeData Data { get; set; }

        public class TemplateTypeData
        {
            public string TemplateTypeName { get; set; }
            public string TemplateTypeId { get; set; }
        }

    }
}
