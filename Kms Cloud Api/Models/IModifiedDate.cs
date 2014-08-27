using System;

namespace Kms.Cloud.Api.Models {
    public abstract class IModifiedDate {
        public virtual DateTime LastModified {
            get;
            set;
        }
    }
}