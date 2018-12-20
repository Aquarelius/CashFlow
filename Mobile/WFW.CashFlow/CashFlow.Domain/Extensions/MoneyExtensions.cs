using System;
using System.Globalization;

namespace CashFlow.Domain.Extensions
{
    public static class MoneyExtensions
    {
        public static string ToMoneyString(this double value)
        {
            return Math.Round(value, 2).ToString(CultureInfo.InvariantCulture);
        }

        public static string ToMoneyString(this double value, string currency)
        {
            return Math.Round(value, 2).ToString(CultureInfo.InvariantCulture) + " " + currency; 
        }

        public static string ToPercentageString(this double value)
        {
            return Math.Round(value, 1).ToString(CultureInfo.InvariantCulture) + " %";
        }
    }
}
