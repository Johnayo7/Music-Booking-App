
using System.ComponentModel;

namespace Music_Booking_App.Models.RequestModels
{
    public class LoginRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        [DefaultValue(false)]
        public bool RememberMe { get; set; }
    }
}
