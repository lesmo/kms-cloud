using KilometrosDatabase.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase {
    public partial class UserBody {
        public double CalculateCaloriesBurned(int steps, DataActivity activity) {
            if ( steps < 1 && activity != DataActivity.Sleep )
                return 0;

            double met
                = 0;

            switch ( activity ) {
                case DataActivity.Walking:
                    met = 3.0d;
                    break;
                case DataActivity.Running:
                    met = 7.0d;
                    break;
                case DataActivity.Sleep:
                    if ( steps == 0 )
                        met = 1.0d;
                    else
                        met = 1.2d;
                    break;
            }

            return met * this.Weight.GramsToKilograms() * 0.033333333333333;
        }

        public double RestMetabolicRate {
            get {
                // Fórmula de Cunningham
                // TODO: Sustituir 1.7 con PAL (Physical Activity Index) calculado a partir
                //       de datos obtenidos por KMS.
                return (500d + 22d * this.LeanBodyMass) * 1.6d;
            }
        }

        /// <summary>
        ///     Lean Body Mass en Kilogramos
        /// </summary>
        public double LeanBodyMass {
            get {
                if ( this.Sex == "m" )
                    return 0.3281 * this.Weight.GramsToKilograms() + 0.33929 * this.Height - 29.5336;
                else
                    return 0.29569 * this.Weight.GramsToKilograms() + 0.41813 * this.Height - 43.2933;
            }
        }
    }
}
