using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.WebApp.Models.Views {
    public class SearchValues {
        public String SearchString {
            get;
            set;
        }

        public Int32 ResultsPages {
            get;
            set;
        }

        public IEnumerable<FriendModel> Results {
            get;
            set;
        }
    }
}