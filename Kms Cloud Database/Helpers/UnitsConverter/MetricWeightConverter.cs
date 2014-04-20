using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.Helpers {
    public static partial class UnitsConverter {
        public static Double GramsToKilograms(this IConvertible @this) {
            return @this.ToDouble() / 1000d;
        }

        public static Double KilogramsToGrams(this IConvertible @this) {
            return @this.ToDouble() * 1000d;
        }
    }
}
