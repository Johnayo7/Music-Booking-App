using Music_Booking_App.Core.Attibutes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Booking_App.Models.Entiites
{
    [Table("BookingPayments")]
    [ReadTableName("public.\"BookingPayments\"")]
    [WriteTableName("public.\"BookingPayments\"")]
    public class BookingPayment : BaseEntity
    {
        public Guid BookingId { get; set; }
        public string EventName { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceId { get; set; }
        public string PaymentStatus { get; set; }
        public string FeeType { get; set; }
        public string ReferenceUrl { get; set; }
        public string OrganizerName { get; set; }
        public Guid EventOrganizerId { get; set; }
        public string ArtisteName { get; set; }
        public Guid ArtisteId { get; set; }
    }
}
