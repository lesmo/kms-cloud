using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Models.Views {
    public class FriendsValues {
        public FriendModel[] FriendRequests {
            get;
            set;
        }

        public FriendModel[] Friends {
            get;
            set;
        }

        public int TotalFriends {
            get;
            set;
        }

        public int TotalPages {
            get;
            set;
        }
    }
}