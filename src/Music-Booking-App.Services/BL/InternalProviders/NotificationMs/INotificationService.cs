

namespace Music_Booking_App.Services.BL.InternalProviders.NotificationMs
{
    public interface INotificationService
    {
        Task SendEmailAsync(string recipientEmail, string templateName, string value);
    }
}
