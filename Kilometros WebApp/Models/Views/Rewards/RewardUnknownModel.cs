using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Models.Views {
    public class RewardUnknownModel {
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

        public long TriggerDistanceCentimeters {
            get;
            set;
        }

        public double TriggerDistance {
            get {
                if ( this.RegionInfo.IsMetric )
                    return this.TriggerDistanceCentimeters / 1000;
                else
                    return (double)(this.TriggerDistanceCentimeters / 160934.4);
            }
        }

        public string TriggerDistanceString {
            get {
                return this.TriggerDistance.ToString(
                    this.CultureInfo.NumberFormat
                );
            }
        }

        public double RemainingDistanceCentimeters {
            get;
            set;
        }

        public double RemainingDistance {
            get {
                if ( this.RegionInfo.IsMetric )
                    return this.RemainingDistanceCentimeters / 1000;
                else
                    return (double)(this.RemainingDistanceCentimeters / 160934.4);
            }
        }

        public string RemainingDistanceString {
            get {
                return this.RemainingDistance.ToString(
                    this.CultureInfo.NumberFormat
                );
            }
        }
    }
}