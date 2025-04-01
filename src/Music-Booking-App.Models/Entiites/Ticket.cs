using Music_Booking_App.Core.Attibutes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Booking_App.Models.Entiites
{
    [Table("Tickets")]
    [ReadTableName("public.\"Tickets\"")]
    [WriteTableName("public.\"Tickets\"")]
    public class Ticket : BaseEntity
    {
        public Guid EventId { get; set; }
        public Guid BuyerId { get; set; }
        public string EventName { get; set; }
        public string BuyerName { get; set; }
        public decimal AmountPaid { get; set; }
        public int NoOfTickets { get; set; }
        public string Status { get; set; }
    }
}
