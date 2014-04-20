using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class RewardResponse {
        public string RewardId { get; set; }
        public DateTime EarnDate { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }

        public RewardGiftResponse[] RewardGifts { get; set; }
        public string[] RegionCodes { get; set; }
    }
}