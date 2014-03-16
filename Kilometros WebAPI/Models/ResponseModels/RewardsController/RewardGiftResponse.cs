using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.RewardsController {
    public class RewardGiftResponse {
        public string RewardGiftId;

        public int Stock;
        public string NameSingular;
        public string NamePlural;
        public bool Claimed;

        public string[] Pictures;
    }
}