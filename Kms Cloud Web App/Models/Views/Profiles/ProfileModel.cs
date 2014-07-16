using Kms.Cloud.Database;
using Kms.Cloud.WebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.WebApp.Models.Views {
    public class ProfileModel : FriendModel {
        public ProfileModel(User user, BaseController controller)
            : base(user, controller) {
            var countryCode = user.RegionCode.Remove(2);
            var region      = controller.Database.RegionStore.GetFirst(
                filter: f =>
                    f.IsoCode == countryCode
            );

            this.Location = region == null
                ? user.RegionCode
                : region.Name;
        }

        public String Location {
            get;
            set;
        }

        public IEnumerable<RewardModel> UnlockedRewards {
            get;
            set;
        }
    }
}