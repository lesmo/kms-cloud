using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase {
    public partial class WristBand {
        public long GenerateId() {
            Guid guid  = Guid.NewGuid();
            long crc64 = (long)Helpers.Crc64Iso.Compute(guid.ToByteArray());

            crc64 = Math.Abs(crc64);
            this.Id = crc64;

            return this.Id;
        }

        public string GenerateIdString() {
            return Helpers.Base36Encoder.Encode(this.Id);
        }

        public string IdString {
            get {
                if ( this._idString == null ) {
                    this._idString
                        = Helpers.Base36Encoder.Encode(this.Id);
                }

                return this._idString;
            }
        }
        private string _idString;
    }
}
