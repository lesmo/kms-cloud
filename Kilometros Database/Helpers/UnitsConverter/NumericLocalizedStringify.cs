using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Helpers {
    public static partial class UnitsConverter {
        private static Func<dynamic, CultureInfo, string> NumberToStringFunction
            = (f, c) => ((Double)f).ToString(
                "N",
                (c ?? CultureInfo.CurrentCulture).NumberFormat
            );
        private static Func<dynamic, CultureInfo, string> CurrencyToStringFunction
            = (f, c) => ((Double)f).ToString(
                "C1",
                (c ?? CultureInfo.CurrentCulture).NumberFormat
            );

        public static string ToLocalizedString(this Double @this, CultureInfo culture = null) {
            return NumberToStringFunction(@this, culture);
        }
        public static string ToLocalizedString(this Int64 @this, CultureInfo culture = null) {
            return NumberToStringFunction(@this, culture);
        }
        public static string ToLocalizedString(this Int32 @this, CultureInfo culture = null) {
            return NumberToStringFunction(@this, culture);
        }
        public static string ToLocalizedString(this Int16 @this, CultureInfo culture = null) {
            return NumberToStringFunction(@this, culture);
        }

        public static string ToCurrencyString(this Double @this, CultureInfo culture = null) {
            return CurrencyToStringFunction(@this, culture);
        }
        public static string ToCurrencyString(this Int64 @this, CultureInfo culture = null) {
            return CurrencyToStringFunction(@this, culture);
        }
        public static string ToCurrencyString(this Int32 @this, CultureInfo culture = null) {
            return CurrencyToStringFunction(@this, culture);
        }
        public static string ToCurrencyString(this Int16 @this, CultureInfo culture = null) {
            return CurrencyToStringFunction(@this, culture);
        }
    }
}
