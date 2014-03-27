using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.ResponseModels {
    public class RewardGiftResponse {
        public string RewardGiftId { get; set; }

        public int Stock { get; set; }
        public string NameSingular { get; set; }
        public string NamePlural { get; set; }
        public bool Claimed { get; set; }

        public string[] Pictures { get; set; }
    }
}