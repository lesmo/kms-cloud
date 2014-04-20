using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.WebApp.Models.Views {
    public class RewardsValues {
        public RewardUnknownModel[] NextRewards {
            get;
            set;
        }

        public RewardModel[] UnlockedRewards {
            get;
            set;
        }

        public int TotalPages {
            get;
            set;
        }
    }
}