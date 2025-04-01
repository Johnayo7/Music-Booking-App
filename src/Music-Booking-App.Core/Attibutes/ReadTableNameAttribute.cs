

namespace Music_Booking_App.Core.Attibutes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReadTableNameAttribute : Attribute
    {
        public ReadTableNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
