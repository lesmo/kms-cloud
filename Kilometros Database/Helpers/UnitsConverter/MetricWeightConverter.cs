using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Helpers {
    public static partial class UnitsConverter {
        private static Func<dynamic, Double> GramsToKilogramsFunction
            = f => (Double)f / 1000;
        private static Func<dynamic, Double> KilogramsToGramsFunction
            = f => (Double)f * 1000;

        public static Double GramsToKilograms(this Double @this) {
            return GramsToKilogramsFunction(@this);
        }

        public static Double GramsToKilograms(this Int64 @this) {
            return GramsToKilogramsFunction(@this);
        }

        public static Double GramsToKilograms(this Int32 @this) {
            return GramsToKilogramsFunction(@this);
        }

        public static Double GramsToKilograms(this Int16 @this) {
            return GramsToKilogramsFunction(@this);
        }

        public static Double KilogramsToGrams(this Double @this) {
            return KilogramsToGramsFunction(@this);
        }

        public static Double KilogramsToGrams(this Int64 @this) {
            return KilogramsToGramsFunction(@this);
        }
        public static Double KilogramsToGrams(this Int32 @this) {
            return KilogramsToGramsFunction(@this);
        }

        public static Double KilogramsToGrams(this Int16 @this) {
            return KilogramsToGramsFunction(@this);
        }
    }
}
