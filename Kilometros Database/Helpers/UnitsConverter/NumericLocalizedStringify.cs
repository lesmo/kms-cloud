using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Helpers.UnitsConverter {
    public static partial class UnitsConverter {
        private static Func<dynamic, string> NumberToStringFunction
            = f => ((Double)f).ToString("N", CultureInfo.CurrentCulture.NumberFormat);
        private static Func<dynamic, string> CurrencyToStringFunction
            = f => ((Double)f).ToString("C1", CultureInfo.CurrentCulture.NumberFormat);

        public string ToLocalizedString(this Double @this) {
            return NumberToStringFunction(@this);
        }
        public string ToLocalizedString(this Int64 @this) {
            return NumberToStringFunction(@this);
        }
        public string ToLocalizedString(this Int32 @this) {
            return NumberToStringFunction(@this);
        }
        public string ToLocalizedString(this Int16 @this) {
            return NumberToStringFunction(@this);
        }

        public string ToCurrencyString(this Double @this) {
            return CurrencyToStringFunction(@this);
        }
        public string ToCurrencyString(this Int64 @this) {
            return CurrencyToStringFunction(@this);
        }
        public string ToCurrencyString(this Int32 @this) {
            return CurrencyToStringFunction(@this);
        }
        public string ToCurrencyString(this Int16 @this) {
            return CurrencyToStringFunction(@this);
        }
    }
}
