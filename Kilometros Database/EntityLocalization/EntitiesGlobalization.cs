using KilometrosDatabase.EntityLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase {
    public partial class Tip : IEntityGlobalization<TipGlobalization> { }
    public partial class TipCategory : IEntityGlobalization<TipCategoryGlobalization> { }

    public partial class Reward : IEntityGlobalization<RewardGlobalization> { }
    public partial class RewardGift : IEntityGlobalization<RewardGiftGlobalization> { }

    public partial class MotionLevel : IEntityGlobalization<MotionLevelGlobalization> { }
}
