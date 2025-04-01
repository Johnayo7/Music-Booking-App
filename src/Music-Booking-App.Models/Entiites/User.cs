using Music_Booking_App.Core.Attibutes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Booking_App.Models.Entiites
{
    [Table("Users")]
    [ReadTableName("public.\"Users\"")]
    [WriteTableName("public.\"Users\"")]
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string OrganizationName { get; set; }
        public string DefaultPassword { get; set; }
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordSalt { get; set; } = new byte[0];
        public string Name { get; set; }
        public bool IsPasswordUpdated { get; set; }
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsDeactivated { get; set; }
        public string UserCategory { get; set; }


        public DateTime? LastLoginDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string SecurityStamp { get; set; }

        public Guid? OrganizationId { get; set; }
        public string ApplicantId { get; set; }
        public string ExternalUserId { get; set; }

        // Account status property
        public string AccountStatus { get; set; } //= Enums.AccountStatus.Pending.ToString();

        // Lockout properties
        public int AccessFailedCount { get; set; } = 0;
        public bool LockoutEnabled { get; set; } = true;
        public DateTime LockOutStart { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
