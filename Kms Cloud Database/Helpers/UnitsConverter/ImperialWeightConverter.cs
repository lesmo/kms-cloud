using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.Helpers {
    public static partial class UnitsConverter {
        private const Double GramsPerPound
            = 453.59237d;
        private const Double OuncesPerPound
            = 16;

        public static Double GramsToPounds(this IConvertible @this) {
            return @this.ToDouble() / GramsPerPound;
        }

        public static Double PoundsToGrams(this IConvertible @this) {
            return @this.ToDouble() * GramsPerPound;
        }
    }
}
