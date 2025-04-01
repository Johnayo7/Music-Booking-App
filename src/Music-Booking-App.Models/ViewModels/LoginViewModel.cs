

namespace Music_Booking_App.Models.ViewModels
{
    public class LoginViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string AccountStatus { get; set; }
        // public bool IsSuperAdmin { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
