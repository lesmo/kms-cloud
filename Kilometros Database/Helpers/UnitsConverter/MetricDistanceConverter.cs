using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase.Helpers {
    public static partial class UnitsConverter {
        private static Func<dynamic, Double> CentimetersToMetersFunction
            = f => (Double)f / 100d;
        private static Func<dynamic, Double> MetersToKilometersFunction
            = f => (Double)f / 1000d;

        public static Double CentimetersToMeters(this Double @this) {
            return CentimetersToMetersFunction(@this);
        }

        public static Double CentimetersToMeters(this Int64 @this) {
            return CentimetersToMetersFunction(@this);
        }

        public static Double CentimetersToMeters(this Int32 @this) {
            return CentimetersToMetersFunction(@this);
        }

        public static Double CentimetersToMeters(this Int16 @this) {
            return CentimetersToMetersFunction(@this);
        }

        public static Double CentimetersToKilometers(this Double @this) {
            return @this.CentimetersToMeters().MetersToKilometers();
        }

        public static Double CentimetersToKilometers(this Int64 @this) {
            return @this.CentimetersToMeters().MetersToKilometers();
        }

        public static Double CentimetersToKilometers(this Int32 @this) {
            return @this.CentimetersToMeters().MetersToKilometers();
        }

        public static Double CentimetersToKilometers(this Int16 @this) {
            return @this.CentimetersToMeters().MetersToKilometers();
        }

        public static Double MetersToKilometers(this Double @this) {
            return MetersToKilometersFunction(@this);
        }

        public static Double MetersToKilometers(this Int64 @this) {
            return MetersToKilometersFunction(@this);
        }

        public static Double MetersToKilometers(this Int32 @this) {
            return MetersToKilometersFunction(@this);
        }

        public static Double MetersToKilometers(this Int16 @this) {
            return MetersToKilometersFunction(@this);
        }
    }
}
