using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kilometros_WebApp.Models.Views.Tips {
    public class TipsValues {
        public TipModel TipOfTheDay {
            get;
            set;
        }

        public TipCategoryModel CurrentCategory {
            get;
            set;
        }

        public TipCategoryModel[] Categories {
            get;
            set;
        }

        public TipModel[] CurrentCategoryTips {
            get;
            set;
        }
    }
}