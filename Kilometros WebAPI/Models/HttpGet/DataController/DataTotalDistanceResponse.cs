using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.DataController {
    public class DataTotalDistanceResponse {
        public long RunningTotalDistance;
        public long WalkingTotalDistance;
        public long TotalDistance;

        public DateTime LastModified;
    }
}