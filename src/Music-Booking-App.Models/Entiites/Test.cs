
using Music_Booking_App.Core.Attibutes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Booking_App.Models.Entiites
{
    [Table("Tests")]
    [ReadTableName("public.\"Tests\"")]
    [WriteTableName("public.\"Tests\"")]
    public class Test : BaseEntity
    {
        public string Name { get; set; }
    }
}
