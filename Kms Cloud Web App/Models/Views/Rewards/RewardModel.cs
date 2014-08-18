using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Kms.Cloud.Database;
using Kms.Cloud.Database.Abstraction;
using Kms.Cloud.WebApp.Controllers;

namespace Kms.Cloud.WebApp.Models.Views {
    public class RewardModel : RewardUnknownModel {
        public RewardModel() {
        }

        public RewardModel(UserEarnedReward earnedReward, BaseController controller) {
            RewardId = earnedReward.Guid.ToBase64String();
            IconUri  = controller.GetDynamicResourceUri(earnedReward.Reward);

            var globalization = earnedReward.Reward.GetGlobalization();
            Title = globalization.Title;
            Text  = globalization.Text;

            UnlockDate = earnedReward.CreationDate;

            if ( earnedReward.Reward.RewardSponsor == null )
                return;
            
            SponsorName = earnedReward.Reward.RewardSponsor.Name;
            SponsorIcon = controller.GetDynamicResourceUri(earnedReward.Reward.RewardSponsor);
            SponsorUri  = new Uri(earnedReward.Reward.RewardSponsor.WebsiteUri);
            Discarded   = earnedReward.Discarded;
        }
        
        public string RewardId {
            get;
            set;
        }

        public Uri IconUri {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }

        public string Text {
            get;
            set;
        }

        public DateTime UnlockDate {
            get;
            set;
        }

        public string SponsorName {
            get;
            set;
        }

        public Uri SponsorIcon {
            get;
            set;
        }

        public Uri SponsorUri { get; set; }

        public Boolean Discarded { get; set; }
    }
}