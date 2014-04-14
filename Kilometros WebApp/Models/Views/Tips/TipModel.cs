﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Models.Views.Tips {
    public class TipModel {
        public string TipId {
            get;
            set;
        }

        public string Text {
            get;
            set;
        }

        public string Category {
            get;
            set;
        }

        public Uri IconUri {
            get;
            set;
        }
    }
}