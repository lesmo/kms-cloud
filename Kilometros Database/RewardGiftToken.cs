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
    
    public partial class RewardGiftToken
    {
        public System.Guid Guid { get; set; }
        public System.DateTime CreationDate { get; internal set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string RedeemCode { get; set; }
        public byte[] RedeemGraphic { get; set; }
        public string RedeemGraphicMimeType { get; set; }
    
        public virtual RewardGift RewardGift { get; set; }
        public virtual User RedeemedByUser { get; set; }
    }
}
