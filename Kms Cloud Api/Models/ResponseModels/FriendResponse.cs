﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.Api.Models.ResponseModels {
    public class FriendResponse {
        public string UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public Uri PictureUri { get; set; }
    }
}