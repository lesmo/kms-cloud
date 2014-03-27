using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.RequestModels{
    public class PhysiquePost {
        public short Age { get; set; }
        public short Height { get; set; }
        public int Weight { get; set; }
        public string Sex { get; set; }

        public short StrideLengthRunning { get; set; }
        public short StrideLengthWalking { get; set; }
    }
}