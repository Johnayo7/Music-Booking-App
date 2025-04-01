

using System.ComponentModel;

namespace Music_Booking_App.Models.RequestModels
{
    public class SignUpRequestModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        // [DefaultValue(false)]
        //public bool IsSuperAdmin { get; set; }
        //public string DefaultPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
