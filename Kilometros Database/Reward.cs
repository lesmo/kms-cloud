//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KilometrosDatabase
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reward : IPicture
    {
        public Reward()
        {
            this.RewardGift = new HashSet<RewardGift>();
            this.RewardGlobalization = new HashSet<RewardGlobalization>();
            this.RewardRegionalization = new HashSet<RewardRegionalization>();
            this.UserEarnedReward = new HashSet<UserEarnedReward>();
        }
    
        public long DistanceTrigger { get; set; }
    
        public virtual ICollection<RewardGift> RewardGift { get; set; }
        public virtual ICollection<RewardGlobalization> RewardGlobalization { get; set; }
        public virtual ICollection<RewardRegionalization> RewardRegionalization { get; set; }
        public virtual ICollection<UserEarnedReward> UserEarnedReward { get; set; }
        public virtual RewardSponsor RewardSponsor { get; set; }
    }
}
