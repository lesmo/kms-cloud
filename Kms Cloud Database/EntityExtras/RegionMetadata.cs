using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database {
    public class RegionMetadata {
        public Region Region { get; set; }

        public RegionSubdivision RegionSubdivision { get; set; }

        public RegionParticular RegionParticular { get; set; }
    }
}
