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
    
    public partial class UserEarnedReward
    {
        public System.Guid Guid { get; set; }
        public System.DateTime CreationDate { get; set; }
        public bool Discarded { get; set; }
    
        public virtual User User { get; set; }
        public virtual Reward Reward { get; set; }
    }
}