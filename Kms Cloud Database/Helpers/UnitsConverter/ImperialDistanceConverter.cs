using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database.Helpers {
    public static partial class UnitsConverter {
        private const Double CentimetersPerInch
            = 2.54d;
        private const Int16 InchesPerFoot
            = 12;
        private const Int16 FeetPerYeard
            = 3;
        private const Int16 YardsPerMile
            = 1760;
        private const Double CentimetersPerMile
            = 160934.4d;

        public static Double CentimetersToInches(this IConvertible @this) {
            return @this.ToDouble() / CentimetersPerInch;
        }

        public static Double InchesToCentimeters(this IConvertible @this) {
            return @this.ToDouble() * CentimetersPerInch;
        }

        public static Double CentimetersToMiles(this IConvertible @this) {
            return (Double)@this / CentimetersPerMile;
        }

        public static Double MilesToCentimeters(this IConvertible @this) {
            return (Double)@this * CentimetersPerMile;
        }
    }
}
