
using Music_Booking_App.Core.Attibutes;
using System.ComponentModel.DataAnnotations;

namespace Music_Booking_App.Models.Entiites
{
    [Serializable]
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

        [IgnoreDuringInsertOrUpdate]
        [Timestamp]
        public byte[] TimeStamp { get; set; } = null!;
    }
}
