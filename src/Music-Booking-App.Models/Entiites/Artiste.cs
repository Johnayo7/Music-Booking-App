using Music_Booking_App.Core.Attibutes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Booking_App.Models.Entiites
{
    [Table("Artistes")]
    [ReadTableName("public.\"Artistes\"")]
    [WriteTableName("public.\"Artistes\"")]
    public class Artiste : BaseEntity
    {
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Bio { get; set; }
        public decimal BookingRate { get; set; }
        public string AccountStatus { get; set; } = Enums.AccountStatus.Pending.ToString();

        public Guid? CreatedBy { get; set; }

        public string Comment { get; set; }

        public string ReviewerName { get; set; }
        public Guid? ReviewerId { get; set; }
    }
}
