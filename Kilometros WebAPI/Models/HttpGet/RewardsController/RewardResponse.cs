using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.RewardsController {
    public class RewardResponse {
        public string RewardId;
        public DateTime EarnDate;

        public string Title;
        public string Text;
        public string Source;

        public string[] RegionCodes;
    }
}