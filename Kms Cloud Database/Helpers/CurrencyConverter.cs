using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.Helpers {
    public static class CurrencyConverter {
        private static Dictionary<String, Double> DollarEquivalence
            = new Dictionary<String, Double> {
                {"USD", 1},
                {"MXN", 13.0d},
                {"EUR", 0.7d},
                {"GBP", 0.6d}
            };

        public static Double DollarCentsToDollars(this IConvertible @this) {
            return @this.ToDouble() / 100d;
        }

        public static Int32 DollarsToDollarCents(this IConvertible @this) {
            return (Int32)(@this.ToDouble() * 100d);
        }

        public static Double? DollarsToRegionCurrency(this IConvertible @this, RegionInfo region = null) {
            if ( region == null )
                region
                    = RegionInfo.CurrentRegion;

            if ( DollarEquivalence.ContainsKey(region.ISOCurrencySymbol.ToUpper()) )
                return DollarEquivalence[region.ISOCurrencySymbol.ToUpper()] * @this.ToDouble();
            else
                return null;
        }

        public static Double? DollarCentsToRegionCurrency(this IConvertible @this, RegionInfo region = null) {
            return @this.DollarCentsToDollars().DollarsToRegionCurrency(region);
        }
    }
}
