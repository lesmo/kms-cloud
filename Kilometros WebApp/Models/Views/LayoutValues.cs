using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Kilometros_WebApp.Models.Views {    
    public class LayoutValues {
        public enum MainSection {
            Home,
            Tips,
            Compare,
            Rewards,
            MyProfile
        }


        public MainSection Section
            = MainSection.Home;
        public int UnreadMessages
            = 0;

        /// <summary>
        /// Uri de la ubicación de la fotografía del Usuario
        /// </summary>
        public Uri UserPicture
            = new Uri("/Images/Header.DefaultUserIcon.png", UriKind.Relative);
        /// <summary>
        /// Nombre del Usuario
        /// </summary>
        public string UserName
            = "";
        /// <summary>
        /// Apellido del Usuario
        /// </summary>
        public string UserLastname
            = "";

        public RegionInfo RegionInfo {
            get;
            set;
        }
        public CultureInfo CultureInfo {
            get;
            set;
        }

        /// <summary>
        /// Distancia Total recorrida por el Usuario en las unidades de la región del Usuario
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
        /// Distancia Total recorrida por el Usuario en las unidades de la región del Usuario, en el formato de la Cultura del Usuario
        /// </summary>
        public string TotalDistanceString {
            get {
                return this.TotalDistance.ToString(
                    this.CultureInfo.NumberFormat
                );
            }
        }

        /// <summary>
        /// Distancia Total recorrida por el Usuario en Centímetros (establecer este valor, modifica el valor en <see cref="TotalDistance">TotalDistance</see>)
        /// </summary>
        public long TotalDistanceCentimeters {
            get;
            set;
        }

        /// <summary>
        /// Distancia en Kilómetros restantes para liberar la próxima recompensa
        /// </summary>
        public double NextRewardsDistanceSpan {
            get {
                if ( this.RegionInfo.IsMetric )
                    return (this.NextRewardDistanceCentimeters - this.TotalDistanceCentimeters) / 1000;
                else
                    return (this.NextRewardDistanceCentimeters - this.TotalDistanceCentimeters) / 160934.4;
            }
        }

        /// <summary>
        /// Distancia en Millas en la cual se liberará la próxima recompensa
        /// </summary>
        public long NextRewardDistanceCentimeters {
            get;
            set;
        }

        /// <summary>
        /// Cadena que representa el lugar geográfico del usuario (País, Estado, Municipio)
        /// </summary>
        public string LocationString;
    }
}