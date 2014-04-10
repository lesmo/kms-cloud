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
        /// Distancia Restante para liberar la próxima recompensa
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
        /// Distancia Restante Distancia Restante para liberar la próxima recompensa en las unidades de la región del Usuario, en el formato de la Cultura del Usuario
        /// </summary>
        public string NextRewardsDistanceSpanString {
            get {
                return this.NextRewardsDistanceSpan.ToString(
                    this.CultureInfo.NumberFormat
                );
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
        /// Colección de las últimas recompensas obtenidas por el Usuario
        /// </summary>
        public LayoutReward[] LastRewards {
            get {
                return this._lastRewards;
            }
            set {
                this._lastRewards = value;
                
                foreach ( LayoutReward reward in this._lastRewards ) {
                    reward.RegionInfo
                        = this.RegionInfo;
                    reward.CultureInfo
                        = this.CultureInfo;
                }
            }
        }
        private LayoutReward[] _lastRewards;

        /// <summary>
        /// Colección del Top 5 de Amigos
        /// </summary>
        public LayoutFriend[] Top5Friends {
            get {
                return this._top5Friends;
            }
            set {
                this._top5Friends = value;

                foreach ( LayoutFriend friend in this._top5Friends ) {
                    friend.RegionInfo
                        = this.RegionInfo;
                    friend.CultureInfo
                        = this.CultureInfo;
                }
            }
        }
        private LayoutFriend[] _top5Friends;

        /// <summary>
        /// Número total de Amigos
        /// </summary>
        public int TotalFriends {
            get;
            set;
        }
    }
}