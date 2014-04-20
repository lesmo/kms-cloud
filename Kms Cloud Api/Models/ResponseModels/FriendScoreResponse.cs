using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class FriendScoreResponse {
        public FriendResponse Friend { get; set; }
        public long TotalDistance { get; set; }
        public bool IsMe { get; set; }
    }
}