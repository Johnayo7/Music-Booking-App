

namespace Music_Booking_App.Core.Constants
{
    public static class CurrencyConstant
    {
        private const int NairaToKoboConversionFactor = 100;

        public static long ConvertNairaToKobo(decimal amount)
        {
            return (long)(amount * NairaToKoboConversionFactor);
        }

        public static decimal ConvertKoboToNaira(long amountInKobo)
        {
            return Math.Round((decimal)amountInKobo / NairaToKoboConversionFactor, 2);
        }
    }
}
