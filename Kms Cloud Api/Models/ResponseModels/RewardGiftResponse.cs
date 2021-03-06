﻿using System.Collections.Generic;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class RewardGiftResponse {
        public string RewardGiftId { get; set; }

        public int Stock { get; set; }
        public string NameSingular { get; set; }
        public string NamePlural { get; set; }
        public bool Claimed { get; set; }

        public IEnumerable<string> Pictures { get; set; }
    }
}