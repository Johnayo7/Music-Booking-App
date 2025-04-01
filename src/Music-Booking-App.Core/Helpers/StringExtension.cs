
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Music_Booking_App.Core.Helpers
{
    public static class StringExtension
    {
        private static readonly string symbol = "@#$&_";
        private static readonly string alphaNumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Encode(this string str, int? strLenght = null)
        {
            var strByte = Encoding.UTF8.GetBytes(str);
            var encode = Convert.ToBase64String(strByte);
            if (strLenght.HasValue)
            {
                encode = encode.Take(strLenght.Value);
            }
            return encode;
        }

        public static string Decode(this string str)
        {
            var encode = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(encode);
        }

        public static string Take(this string str, int strLenght)
        {
            if (strLenght > str.Length)
            {
                strLenght = str.Length;
            }
            return str.Substring(0, strLenght);
        }

        public static byte[] GetBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string ToJsonString(this object jsonObj)
        {
            return JsonSerializer.Serialize(jsonObj);
        }

        public static string GenerateRandomString(int maxLenght, int byteLength = 8)
        {
            var randomNumber = new byte[byteLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return randomNumber.ToBase64String()
                             .Take(maxLenght);
        }

        public static string GenerateAlhaNumericString(int charLenght)
        {
            string allowedChars = alphaNumeric + symbol;
            Random random = new();
            string str = new(Enumerable.Repeat(allowedChars, charLenght)
                                                      .Select(s => s[random.Next(s.Length)]).ToArray());

            if (!IsStrongPassword(str, out var _))
            {
                return GenerateAlhaNumericString(charLenght);
            }
            return str;
        }

        public static bool IsStrongPassword(string password, out string msg)
        {
            msg = null;
            bool isAlphanumeric = password.Any(char.IsUpper) && password.Any(char.IsLower)
                          && password.Any(char.IsDigit);
            bool containsNonAlphanumeric = symbol.Any(password.Contains);

            if (!containsNonAlphanumeric && !isAlphanumeric)
            {
                msg = "Password must Alphanumeric with symbol";
                return false;
            }

            if (!containsNonAlphanumeric)
            {
                msg = "Password must contain at least one of this symbol '@#$&_'";
                return false;
            }

            if (!isAlphanumeric)
            {
                msg = "Password must contain at least one upper, lower and a number";
                return false;
            }

            return true;
        }

        public static string RemoveNonAlphaNumeric(this string str, int? strLenght = null)
        {
            str = Regex.Replace(str, "[^a-zA-Z0-9]", "");
            if (strLenght is not null)
            {
                str = str.Take(strLenght.Value);
            }
            return str;
        }

        public static string AppendNonAlphaNumeric(this string str, int num)
        {
            num = num > 6 ? 6 : num;
            var allowedChars = "@#$%&_";
            Random random = new();
            string randomChars = new(Enumerable.Repeat(allowedChars, num)
                                                .Select(s => s[random.Next(s.Length)]).ToArray());
            return str + randomChars;
        }
    }
}
