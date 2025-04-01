namespace Music_Booking_App.Services.Authentication.Configurations
{
    public class JwtConfig
    {
        public string SecretKey { get; set; }
        public int TokenLifeTime { get; set; }
        public int RefreshTokenLiftTime { get; set; }
    }
}
