
using System.Dynamic;

namespace Music_Booking_App.Core.Helpers
{
    public static class DictionaryToObjectConverter
    {
        public static object ConvertToAnonymousObject(this Dictionary<string, object> dict)
        {
            var eo = new ExpandoObject();
            var eoColl = (ICollection<KeyValuePair<string, object>>)eo;

            foreach (var kvp in dict) eoColl.Add(kvp);
            dynamic eoDynamic = eo;
            return eoDynamic;
        }
    }
}
