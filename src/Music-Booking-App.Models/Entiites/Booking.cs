using Music_Booking_App.Core.Attibutes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Booking_App.Models.Entiites
{
    [Table("Bookings")]
    [ReadTableName("public.\"Bookings\"")]
    [WriteTableName("public.\"Bookings\"")]
    public class Booking : BaseEntity
    {
        public Guid ArtisteId { get; set; }
        public Guid EventId { get; set; }
        public Guid EventOrganizerId { get; set; }
        public string ArtisteName { get; set; }
        public string EventName { get; set; }
        public string OrganizerName { get; set; }
        public decimal ProposedAmount { get; set; }
        public string Status { get; set; }
    }
}
