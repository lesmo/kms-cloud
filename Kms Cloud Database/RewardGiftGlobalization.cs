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
    
    public partial class RewardGiftGlobalization : IGlobalization
    {
        public string NameSingular { get; set; }
        public string NamePlural { get; set; }
        public string Description { get; set; }
    
        public virtual RewardGift RewardGift { get; set; }
    }
}
