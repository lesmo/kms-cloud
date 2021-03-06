//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kms.Cloud.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.Token = new HashSet<Token>();
            this.UserRewardGiftClaimed = new HashSet<UserRewardGiftClaimed>();
            this.UserMotionLevelHistory = new HashSet<UserMotionLevelHistory>();
            this.UserEarnedReward = new HashSet<UserEarnedReward>();
            this.Data = new HashSet<Data>();
            this.UserTipHistory = new HashSet<UserTipHistory>();
            this.OAuthCredential = new HashSet<OAuthCredential>();
            this.UserFriended = new HashSet<UserFriend>();
            this.UserFriend = new HashSet<UserFriend>();
            this.UserDataTotalDistance = new HashSet<UserDataTotalDistance>();
            this.UserDataHourlyDistance = new HashSet<UserDataHourlyDistance>();
            this.Notification = new HashSet<Notification>();
            this.Device = new HashSet<Device>();
        }
    
        public System.Guid Guid { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string Email { get; set; }
        internal byte[] Password { get; set; }
        internal byte[] PasswordSalt { get; set; }
        public string RegionCode { get; set; }
        public string PreferredCultureCode { get; set; }
        public Nullable<short> UtcOffset { get; set; }
        public System.DateTime BirthDate { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PictureUri { get; set; }
    
        public virtual ICollection<Token> Token { get; set; }
        public virtual UserBody UserBody { get; set; }
        public virtual ICollection<UserRewardGiftClaimed> UserRewardGiftClaimed { get; set; }
        public virtual ICollection<UserMotionLevelHistory> UserMotionLevelHistory { get; set; }
        public virtual ICollection<UserEarnedReward> UserEarnedReward { get; set; }
        public virtual ContactInfo ContactInfo { get; set; }
        public virtual ShippingInformation ShippingInformation { get; set; }
        public virtual ICollection<Data> Data { get; set; }
        public virtual ICollection<UserTipHistory> UserTipHistory { get; set; }
        public virtual ICollection<OAuthCredential> OAuthCredential { get; set; }
        public virtual ICollection<UserFriend> UserFriended { get; set; }
        public virtual ICollection<UserFriend> UserFriend { get; set; }
        public virtual ICollection<UserDataTotalDistance> UserDataTotalDistance { get; set; }
        public virtual ICollection<UserDataHourlyDistance> UserDataHourlyDistance { get; set; }
        public virtual ICollection<Notification> Notification { get; set; }
        public virtual UserPicture UserPicture { get; set; }
        public virtual ICollection<Device> Device { get; set; }
    }
}
