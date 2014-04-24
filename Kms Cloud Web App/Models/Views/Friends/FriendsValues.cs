using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.WebApp.Models.Views {
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
    }
}