using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kms.Cloud.WebApp.Models.Views {
    public class TipsValues {
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

        public int CurrentCategoryTipsTotalPages {
            get;
            set;
        }
    }
}