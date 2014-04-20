using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kms.Cloud.Database {
    public partial class RewardGift {
        public int Stock {
            get {
                return this.UserRewardGiftClaimed.Count;
            }
        }
    }
}
