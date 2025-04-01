

using System.Text.Json.Serialization;

namespace Music_Booking_App.Models.RequestModels
{
    public class UpdateTestRequestModel : CreateTestRequestModel
    {
        [JsonIgnore] public Guid Id { get; set; }
    }
}
