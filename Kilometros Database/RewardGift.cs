//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KilometrosDatabase
{
    using System;
    using System.Collections.Generic;
    
    public partial class RewardGift
    {
        public RewardGift()
        {
            this.RewardGiftGlobalization = new HashSet<RewardGiftGlobalization>();
            this.RewardGiftToken = new HashSet<RewardGiftToken>();
            this.RewardGiftImage = new HashSet<RewardGiftImage>();
        }
    
        public System.Guid Guid { get; set; }
    
        public virtual Reward Reward { get; set; }
        public virtual ICollection<RewardGiftGlobalization> RewardGiftGlobalization { get; set; }
        public virtual ICollection<RewardGiftToken> RewardGiftToken { get; set; }
        public virtual ICollection<RewardGiftImage> RewardGiftImage { get; set; }
    }
}
