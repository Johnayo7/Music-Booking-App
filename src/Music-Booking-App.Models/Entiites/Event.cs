using Music_Booking_App.Core.Attibutes;
using Music_Booking_App.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Booking_App.Models.Entiites
{
    [Table("Events")]
    [ReadTableName("public.\"Events\"")]
    [WriteTableName("public.\"Events\"")]
    public class Event : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public decimal TicketPrice { get; set; }
        public string OrganizerName { get; set; }

        public string EventStatus { get; set; } = AccountStatus.Pending.ToString();
        public string Comment { get; set; }

        public Guid EventOrganizerId { get; set; }

        public string ReviewerName { get; set; }
        public Guid? ReviewerId { get; set; }
    }
}
