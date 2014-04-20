using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.WebApp.Models.Views {
    public class LayoutNotification {
        public Uri IconUri {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }

        public string Description {
            get;
            set;
        }

        public bool Discarded {
            get;
            set;
        }
    }
}