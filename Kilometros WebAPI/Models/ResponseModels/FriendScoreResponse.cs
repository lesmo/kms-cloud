using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.ResponseModels {
    public class FriendScoreResponse {
        public FriendResponse Friend;
        public long TotalDistance;
        public bool IsMe;
    }
}