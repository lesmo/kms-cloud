using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Models.Views {
    public class LayoutFriend {
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
        /// Nombre del Amigo
        /// </summary>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Apellido del Amigo
        /// </summary>
        public string LastName {
            get;
            set;
        }

        /// <summary>
        /// URI de la fotografía del Amigo
        /// </summary>
        public Uri PictureUri {
            get;
            set;
        }

        /// <summary>
        /// Distancia Total recorrida por el Amigo (en cm)
        /// </summary>
        public double TotalDistanceCentimeters {
            get;
            set;
        }

        /// <summary>
        /// Distancia Total recorrida por el Amigo en las unidades de la región del Usuario
        /// </summary>
        public double TotalDistance {
            get {
                if ( this.RegionInfo.IsMetric )
                    return this.TotalDistanceCentimeters / 1000;
                else
                    return this.TotalDistanceCentimeters / 160934.4;
            }
        }

        /// <summary>
        /// Distancia Total recorrida por el Amigo en las unidades de la región del Usuario, en el formato de la Cultura del Usuario
        /// </summary>
        public string TotalDistanceString {
            get {
                return this.TotalDistance.ToString(
                    this.CultureInfo.NumberFormat
                );
            }
        }
    }
}