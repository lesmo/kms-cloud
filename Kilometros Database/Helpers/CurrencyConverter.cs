using KilometrosDatabase.Helpers.CurrencyConverter;
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
                {"EUR", 0.7d}
            };
        
        private static Func<dynamic, RegionInfo, Double?> DollarsToRegionCurrencyFunction
            = (dollars, region) => (
                DollarEquivalence.ContainsKey(region.ISOCurrencySymbol.ToUpper())
                ? DollarEquivalence[region.ISOCurrencySymbol.ToUpper()] * dollars
                : null
            );

        public static Double? DollarsToRegionCurrency(this Double @this, RegionInfo region = null) {
            if ( region == null )
                region
                    = RegionInfo.CurrentRegion;

            return DollarsToRegionCurrencyFunction(@this, region);
        }

        public static Double? DollarsToRegionCurrency(this Int64 @this, RegionInfo region = null) {
            if ( region == null )
                region
                    = RegionInfo.CurrentRegion;

            return DollarsToRegionCurrencyFunction(@this, region);
        }

        public static Double? DollarsToRegionCurrency(this Int32 @this, RegionInfo region = null) {
            if ( region == null )
                region
                    = RegionInfo.CurrentRegion;

            return DollarsToRegionCurrencyFunction(@this, region);
        }

        public static Double? DollarsToRegionCurrency(this Int16 @this, RegionInfo region = null) {
            if ( region == null )
                region
                    = RegionInfo.CurrentRegion;

            return DollarsToRegionCurrencyFunction(@this, region);
        }
    }
}
