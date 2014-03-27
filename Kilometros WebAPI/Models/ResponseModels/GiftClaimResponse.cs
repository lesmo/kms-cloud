using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.ResponseModels {
    public class GiftClaimResponse {
        public DateTime ExpirationDate { get; set; }
        public string RedeemCode { get; set; }
        public string RedeemPicture { get; set; }
    }
}