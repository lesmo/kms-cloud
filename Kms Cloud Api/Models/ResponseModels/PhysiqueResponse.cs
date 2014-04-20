using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kms.Cloud.Api.Models;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class PhysiqueResponse : IModifiedDate {
        public short Age { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Sex { get; set; }

        public int StrideLengthWalking { get; set; }
        public int StrideLengthRunning { get; set; }
    }
}