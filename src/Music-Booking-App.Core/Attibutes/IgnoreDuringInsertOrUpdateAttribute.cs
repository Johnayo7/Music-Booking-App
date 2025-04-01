

namespace Music_Booking_App.Core.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreDuringInsertOrUpdateAttribute : Attribute
    {
    }
}
