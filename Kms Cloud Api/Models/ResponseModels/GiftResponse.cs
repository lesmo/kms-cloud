using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class GiftResponse {
        public string NameSingular { get; set; }
        public string NamePlural { get; set; }

        public string RedeemCode { get; set; }
        public string RedeemPicture { get; set; }

        public IEnumerable<string> Pictures { get; set; }
    }
}