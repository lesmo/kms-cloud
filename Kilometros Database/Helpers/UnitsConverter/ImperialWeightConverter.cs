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

        public static Double GramsToPounds(this IConvertible @this) {
            return (Double)@this / GramsPerPound;
        }

        public static Double PoundsToGrams(this IConvertible @this) {
            return (Double)@this * GramsPerPound;
        }
    }
}
