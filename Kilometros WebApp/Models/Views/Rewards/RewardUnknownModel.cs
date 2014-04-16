﻿using KilometrosDatabase.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Models.Views {
    public class RewardUnknownModel {
        public long TriggerDistanceCentimeters {
            get;
            set;
        }

        public string TriggerDistance {
            get {
                double triggerDistance
                    = RegionInfo.CurrentRegion.IsMetric
                    ? this.TriggerDistanceCentimeters.CentimetersToKilometers()
                    : this.TriggerDistanceCentimeters.CentimetersToMiles();
                
                return triggerDistance.ToLocalizedString();
            }
        }

        public double RemainingDistanceCentimeters {
            get;
            set;
        }

        public string RemainingDistance {
            get {
                double remainingDistance
                    = RegionInfo.CurrentRegion.IsMetric
                    ? this.RemainingDistanceCentimeters.CentimetersToKilometers()
                    : this.RemainingDistanceCentimeters.CentimetersToMiles();

                return remainingDistance.ToLocalizedString();
            }
        }
    }
}