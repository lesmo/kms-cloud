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
    
    public partial class User
    {
        public User()
        {
            this.Token = new HashSet<Token>();
            this.RewardGiftToken = new HashSet<RewardGiftToken>();
            this.UserMotionLevelHistory = new HashSet<UserMotionLevelHistory>();
            this.UserEarnedReward = new HashSet<UserEarnedReward>();
            this.Data = new HashSet<Data>();
            this.RewardGiftUserClaimed = new HashSet<RewardGiftUserClaimed>();
        }
    
        public System.Guid Guid { get; private set; }
        public System.DateTimeOffset CreationDate { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public string RegionCode { get; set; }
        public string PreferredCultureCode { get; set; }
    
        public virtual ICollection<Token> Token { get; set; }
        public virtual UserBody UserBody { get; set; }
        public virtual ICollection<RewardGiftToken> RewardGiftToken { get; set; }
        public virtual ICollection<UserMotionLevelHistory> UserMotionLevelHistory { get; set; }
        public virtual ICollection<UserEarnedReward> UserEarnedReward { get; set; }
        public virtual ContactInfo ContactInfo { get; set; }
        public virtual ShippingInformation ShippingInformation { get; set; }
        public virtual SocialIdentity SocialIdentity { get; set; }
        public virtual ICollection<Data> Data { get; set; }
        public virtual ICollection<RewardGiftUserClaimed> RewardGiftUserClaimed { get; set; }
    }
}
