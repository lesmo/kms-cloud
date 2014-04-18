using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Helpers {
    public static class CurrencyConverter {
        private static Dictionary<String, Double> DollarEquivalence
            = new Dictionary<String, Double> {
                {"USD", 1},
                {"MXN", 13.0d},
                {"EUR", 0.7d},
                {"GBP", 0.6d}
            };

        private static Double DollarCentsToDollars(this IConvertible @this) {
            return (Double)@this / 100d;
        }

        private static Int32 DollarsToDollarCents(this IConvertible @this) {
            return (Int32)((Double)@this * 100d);
        }

        private static Double? DollarsToRegionCurrency(this IConvertible @this, RegionInfo region = null) {
            if ( region == null )
                region
                    = RegionInfo.CurrentRegion;

            if ( DollarEquivalence.ContainsKey(region.ISOCurrencySymbol.ToUpper()) )
                return DollarEquivalence[region.ISOCurrencySymbol.ToUpper()] * (Double)@this;
            else
                return null;
        }

        public static Double? DollarCentsToRegionCurrency(this IConvertible @this, RegionInfo region = null) {
            return @this.DollarCentsToDollars().DollarsToRegionCurrency(region);
        }
    }
}
