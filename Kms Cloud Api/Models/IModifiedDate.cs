using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models {
    public abstract class IModifiedDate {
        public virtual DateTime LastModified {
            get {
                return this._lastModified;
            }
            set {
                this._lastModified = value;
            }
        }
        private DateTime _lastModified;
    }
}