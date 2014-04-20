using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.WebApp.Models.Views {
    public class TipCategoryModel {
        public string CategoryId {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public Uri IconUri {
            get;
            set;
        }
    }
}