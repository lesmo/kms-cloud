using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Helpers {
    public static partial class UnitsConverter {
        public static string ToLocalizedString(this IFormattable @this, bool forceDecimals, CultureInfo culture = null) {
            return ((Double)@this).ToLocalizedString(culture);
        }

        public static string ToLocalizedString(this IFormattable @this, CultureInfo culture = null) {
            if ( (@this is Double) || (@this is Decimal) || (@this is float) ) {
                if ( (Double)@this - Math.Truncate((Double)@this) != 0 ) {
                    return @this.ToString(
                        "N",
                        (culture ?? CultureInfo.CurrentCulture).NumberFormat
                    );
                }
            }

            return @this.ToString(
                "N0",
                (culture ?? CultureInfo.CurrentCulture).NumberFormat
            );
        }

        public static string ToCurrencyString(this IFormattable @this, bool skipDecimals, CultureInfo culture = null) {
            return @this.ToString(
                "C0",
                (culture ?? CultureInfo.CurrentCulture).NumberFormat
            );
        }

        public static string ToCurrencyString(this IFormattable @this, CultureInfo culture = null) {
            return @this.ToString(
                "C",
                (culture ?? CultureInfo.CurrentCulture).NumberFormat
            );
        }
    }
}
