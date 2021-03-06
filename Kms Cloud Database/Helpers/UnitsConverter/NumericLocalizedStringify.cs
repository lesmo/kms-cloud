﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.Helpers {
    public static partial class UnitsConverter {
        public static string ToLocalizedString(this IConvertible @this, bool forceDecimals, CultureInfo culture = null) {
            if ( forceDecimals )
                return @this.ToDouble().ToLocalizedString(culture);
            else
                return @this.ToLocalizedString(culture);
        }

        public static string ToLocalizedString(this IConvertible @this, CultureInfo culture = null) {
            if ( !(@this is IFormattable) )
                throw new ArgumentException("Object does not implement IFormattable");

            if ( (@this is Double) || (@this is Decimal) || (@this is float) ) {
                Double truncateResult
                    = @this.ToDouble() - Math.Truncate(@this.ToDouble());

                if (
                    truncateResult > 0.00001d
                    || truncateResult < -0.00001d
                ) {
                    return (@this as IFormattable).ToString(
                        "N",
                        (culture ?? CultureInfo.CurrentCulture).NumberFormat
                    );
                }
            }

            return (@this as IFormattable).ToString(
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
