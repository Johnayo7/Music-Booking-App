namespace Music_Booking_App.Services.Authentication.Interfaces
{
    public interface IOtpService
    {
        string GenerateOTP();
        Task SendEmailAsync(string recipientEmail, string subject, string otp);
    }
}
