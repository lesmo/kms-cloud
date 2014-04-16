using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Helpers {
    public static partial class UnitsConverter {
        private const Double GramsPerPound
            = 453.59237d;
        private const Double OuncesPerPound
            = 16;

        public static Func<dynamic, Double> GramsToPoundsFunction
            = f => (Double)f / GramsPerPound;
        public static Func<dynamic, Double> PoundsToGramsFunction
            = f => (Double)f * GramsPerPound;

        public static Func<dynamic, Double> PoundsToOuncesFunction
            = f => (Double)f / OuncesPerPound;
        public static Func<dynamic, Double> OuncesToPoundsFunction
            = f => (Double)f * OuncesPerPound;

        public static Double GramsToPounds(this Double @this) {
            return GramsToPoundsFunction(@this);
        }

        public static Double GramsToPounds(this Int64 @this) {
            return GramsToPoundsFunction(@this);
        }

        public static Double GramsToPounds(this Int32 @this) {
            return GramsToPoundsFunction(@this);
        }

        public static Double GramsToPounds(this Int16 @this) {
            return GramsToPoundsFunction(@this);
        }

        public static Double PoundsToGrams(this Double @this) {
            return PoundsToGramsFunction(@this);
        }

        public static Double PoundsToGrams(this Int64 @this) {
            return PoundsToGramsFunction(@this);
        }

        public static Double PoundsToGrams(this Int32 @this) {
            return PoundsToGramsFunction(@this);
        }

        public static Double PoundsToGrams(this Int16 @this) {
            return PoundsToGramsFunction(@this);
        }
    }
}
