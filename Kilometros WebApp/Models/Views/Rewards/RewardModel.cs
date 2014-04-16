using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Models.Views {
    public class RewardModel : RewardUnknownModel {

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
    }
}