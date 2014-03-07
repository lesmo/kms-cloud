using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet.DataController {
    public class DataTotalResponse {
        public long RunningTotalDistance;
        public long WalkingTotalDistance;
        public long TotalDistance;

        public long RunningTotalSteps;
        public long WalkingTotalSteps;
        public long TotalSteps;

        public DateTime LastModified;
    }
}