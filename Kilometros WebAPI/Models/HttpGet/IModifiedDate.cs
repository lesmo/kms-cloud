using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.HttpGet {
    public abstract class IModifiedDate {
        public virtual DateTime LastModified {
            get {
                return this._lastModified;
            }
            set {
                this._lastModified = value;
            }
        }
        private virtual DateTime _lastModified;
    }
}