using Kms.Cloud.Database;
using Kms.Cloud.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kms.Cloud.WebApp.Models.Views {
    public class FriendModel {
        public FriendModel(User user) {
            this.UserId
                = user.Guid.ToBase64String();

            this.Name
                = user.Name;
            this.LastName
                = user.LastName;
            this.PictureUri
                = new Uri(user.PictureUri);

            this.TotalDistanceCentimeters
                = RegionInfo.CurrentRegion.IsMetric
                ? user.UserDataTotalDistanceSum.TotalDistance.CentimetersToKilometers()
                : user.UserDataTotalDistanceSum.TotalDistance.CentimetersToMiles();
            this.TotalKcal
                = user.UserDataTotalDistanceSum.TotalKcal;
            this.TotalCo2
                = user.UserDataTotalDistanceSum.TotalCo2;
            this.TotalCashRaw
                = user.UserDataTotalDistanceSum.TotalCash;
        }

        public string UserId {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public string LastName {
            get;
            set;
        }

        public Uri PictureUri {
            get;
            set;
        }

        public Double TotalDistanceCentimeters {
            get;
            set;
        }

        public string TotalDistance {
            get {
                Double totalDistance
                    = RegionInfo.CurrentRegion.IsMetric
                    ? this.TotalDistanceCentimeters.CentimetersToKilometers()
                    : this.TotalDistanceCentimeters.CentimetersToMiles();

                return totalDistance.ToLocalizedString();
            }
        }

        public long TotalKcal {
            get;
            set;
        }

        public long TotalCo2 {
            get;
            set;
        }

        public long TotalCashRaw {
            get;
            set;
        }

        public string TotalCash {
            get {
                return this.TotalCashRaw.ToCurrencyString();
            }
        }
    }
}