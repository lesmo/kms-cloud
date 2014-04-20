using Kms.Cloud.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Kms.Cloud.WebApp.Models.Views {    
    public class LayoutValues {
        public enum MainSection {
            MyProfile,
            Tips,
            Rewards,
            Friends
        }

        public LayoutValues() {
            this.UserPicture
                = new Uri("/Images/Header.DefaultUserIcon.png", UriKind.Relative);
        }

        public MainSection Section
            = MainSection.MyProfile;
        public int UnreadMessages
            = 0;

        /// <summary>
        /// Uri de la ubicación de la fotografía del Usuario
        /// </summary>
        public Uri UserPicture {
            get;
            set;
        }

        /// <summary>
        /// Nombre del Usuario
        /// </summary>
        public string UserName {
            get;
            set;
        }

        /// <summary>
        /// Apellido del Usuario
        /// </summary>
        public string UserLastname {
            get;
            set;
        }

        /// <summary>
        /// Distancia Total recorrida por el Usuario en las unidades de la región del Usuario
        /// </summary>
        public string TotalDistance {
            get {
                double totalDistance
                    = RegionInfo.CurrentRegion.IsMetric
                    ? this.TotalDistanceCentimeters.CentimetersToKilometers()
                    : this.TotalDistanceCentimeters.CentimetersToMiles();
                
                return totalDistance.ToLocalizedString();
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
        /// Distancia Restante para liberar la próxima recompensa
        /// </summary>
        public string NextRewardsDistanceRemaining {
            get {
                double rewardDistanceSpan
                    = this.NextRewardDistanceCentimeters - this.TotalDistanceCentimeters;
                rewardDistanceSpan
                    = RegionInfo.CurrentRegion.IsMetric
                    ? rewardDistanceSpan.CentimetersToKilometers()
                    : rewardDistanceSpan.CentimetersToMiles();

                return rewardDistanceSpan.ToLocalizedString();
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
        public string LocationString {
            get;
            set;
        }

        /// <summary>
        /// Cadena de Representación Compacta del GUID de la Recompensa Desbloqueada recientemente.
        /// Si es nula o vacía, no se ha desbloqueado una recompensa recientemente o ya se marcó como
        /// descartada en alguna ocasión anterior por el API o la misma WebApp.
        /// </summary>
        public RewardModel RecentlyUnlockedReward {
            get;
            set;
        }

        /// <summary>
        /// Tip del Día
        /// </summary>
        public TipModel TipOfTheDay {
            get;
            set;
        }

        public LayoutNotification[] Notifications {
            get;
            set;
        }
    }
}