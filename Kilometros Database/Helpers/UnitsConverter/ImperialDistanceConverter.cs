using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase {
    public static partial class UnitsConverter {
        private static Func<dynamic, Double> InchesToCentimetersFunction
            = f => (Double)f * 2.54d;
        private static Func<dynamic, Double> CentimitersToInchesFunction
            = f => (Double)f / 2.54d;

        private static Func<dynamic, Double> FeetToInchesFunction
            = f => (Double)f * 12d;
        private static Func<dynamic, Double> InchesToFeetFunction
            = f => (Double)f / 12d;

        private static Func<dynamic, Double> YardsToFeetFunction
            = f => (Double)f * 3d;
        private static Func<dynamic, Double> FeetToYardsFunction
            = f => (Double)f / 3d;

        private static Func<dynamic, Double> MilesToYardsFunction
            = f => (Double)f * 1760d;
        private static Func<dynamic, Double> YardsToMilesFunction
            = f => (Double)f / 1760d;

        public static Func<dynamic, Double> MilesToCentimetersFunction
            = f => (Double)f * 160934.4d;
        public static Func<dynamic, Double> CentimetersToMilesFunction
            = f => (Double)f / 160934.4d;

        public static Double CentimetersToInches(this Double @this) {
            return CentimitersToInchesFunction(@this);
        }

        public static Double CentimetersToInches(this Int64 @this) {
            return CentimitersToInchesFunction(@this);
        }

        public static Double CentimetersToInches(this Int32 @this) {
            return CentimitersToInchesFunction(@this);
        }
        public static Double CentimetersToInches(this Int16 @this) {
            return CentimitersToInchesFunction(@this);
        }

        public static Double InchesToCentimeters(this Double @this) {
            return InchesToCentimetersFunction(@this);
        }

        public static Double InchesToCentimeters(this Int64 @this) {
            return InchesToCentimetersFunction(@this);
        }

        public static Double InchesToCentimeters(this Int32 @this) {
            return InchesToCentimetersFunction(@this);
        }

        public static Double InchesToCentimeters(this Int16 @this) {
            return InchesToCentimetersFunction(@this);
        }

        public static Double CentimetersToMiles(this Double @this) {
            return CentimetersToMilesFunction(@this);
        }

        public static Double CentimetersToMiles(this Int64 @this) {
            return CentimetersToMilesFunction(@this);
        }

        public static Double CentimetersToMiles(this Int32 @this) {
            return CentimetersToMilesFunction(@this);
        }

        public static Double CentimetersToMiles(this Int16 @this) {
            return CentimetersToMilesFunction(@this);
        }

        public static Double MilesToCentimeters(this Double @this) {
            return MilesToCentimetersFunction(@this);
        }

        public static Double MilesToCentimeters(this Int64 @this) {
            return MilesToCentimetersFunction(@this);
        }

        public static Double MilesToCentimeters(this Int32 @this) {
            return MilesToCentimetersFunction(@this);
        }

        public static Double MilesToCentimeters(this Int16 @this) {
            return MilesToCentimetersFunction(@this);
        }
    }
}
