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
    
    public partial class RewardRegionalization
    {
        public RewardRegionalization()
        {
            this.Exclude = false;
        }
    
        public long Id { get; set; }
        public string RegionCode { get; set; }
        public bool Exclude { get; set; }
    
        public virtual Reward Reward { get; set; }
    }
}