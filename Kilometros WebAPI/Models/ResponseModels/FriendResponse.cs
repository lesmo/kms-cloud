using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebAPI.Models.ResponseModels {
    public class FriendResponse {
        public string UserId;
        public DateTime CreationDate;
        public string Name;
        public string LastName;

        public string PictureUri;
    }
}