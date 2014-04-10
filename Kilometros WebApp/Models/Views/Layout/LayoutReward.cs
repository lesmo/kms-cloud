using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Models.Views {
    public class LayoutReward {
        /// <summary>
        /// Información sobre la Región del Usuario
        /// </summary>
        public RegionInfo RegionInfo {
            get;
            set;
        }

        /// <summary>
        /// Información sobre la Cultura del Usuario
        /// </summary>
        public CultureInfo CultureInfo {
            get;
            set;
        }

        /// <summary>
        /// Título de la Recompensa
        /// </summary>
        public string Title {
            get;
            set;
        }

        /// <summary>
        /// Distancia a la que se liberó la Recompensa (en cm)
        /// </summary>
        public double TriggerDistanceCentimeters {
            get;
            set;
        }

        /// <summary>
        /// Distancia a la que se liberó la Recompensa en las unidades de la región del Usuario
        /// </summary>
        public double TriggerDistance {
            get {
                if ( this.RegionInfo.IsMetric )
                    return this.TriggerDistanceCentimeters / 1000;
                else
                    return this.TriggerDistanceCentimeters / 160934.4;
            }
        }

        /// <summary>
        /// Distancia a la que se liberó la Recompensa en las unidades de la región del Usuario, en el formato de la Cultura del Usuario
        /// </summary>
        public string TotalDistanceString {
            get {
                return this.TriggerDistance.ToString(
                    this.CultureInfo.NumberFormat
                );
            }
        }
    }
}