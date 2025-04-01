using Music_Booking_App.Core.Attibutes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Booking_App.Models.Entiites
{
    [Table("OTPs")]
    [ReadTableName("public.\"OTPs\"")]
    [WriteTableName("public.\"OTPs\"")]
    public class OTP : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
        public bool IsUsed { get; set; }

        public bool IsExpired(int expirationMinutes)
        {
            return DateTime.UtcNow > CreationDate.AddMinutes(expirationMinutes);
        }
    }
}
