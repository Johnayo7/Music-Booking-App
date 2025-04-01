
namespace Music_Booking_App.Core.Attibutes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WriteTableNameAttribute : Attribute
    {
        public WriteTableNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
