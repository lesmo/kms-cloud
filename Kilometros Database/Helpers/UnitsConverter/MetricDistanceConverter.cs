using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Helpers {
    public static partial class UnitsConverter {
        public static Double CentimetersToMeters(this IConvertible @this) {
            return @this.ToDouble() / 100d;
        }

        public static Double CentimetersToKilometers(this IConvertible @this) {
            return @this.CentimetersToMeters().MetersToKilometers();
        }

        public static Double MetersToKilometers(this IConvertible @this) {
            return @this.ToDouble() / 1000d;
        }
    }
}
