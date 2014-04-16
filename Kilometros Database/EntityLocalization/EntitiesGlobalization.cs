using KilometrosDatabase.EntityLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace KilometrosDatabase {
    public partial class Tip : IEntityGlobalization<TipGlobalization> { }
    public partial class IPicture : IEntityGlobalization<IGlobalization> { }

    public partial class TipCategory {
        public new TipCategoryGlobalization GetGlobalization(CultureInfo culture = null) {
            return this.GetGlobalization<TipCategoryGlobalization>(culture);
        }
    }

    public partial class Reward {
        public new RewardGlobalization GetGlobalization(CultureInfo culture = null) {
            return this.GetGlobalization<RewardGlobalization>(culture);
        }
    }

    public partial class RewardGift : IEntityGlobalization<RewardGiftGlobalization> { }
    public partial class MotionLevel : IEntityGlobalization<MotionLevelGlobalization> { }
}
