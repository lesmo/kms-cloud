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
    
    public partial class Device
    {
        public long Id { get; set; }
        public long SerialNumber { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<System.DateTime> LinkDate { get; set; }
        public string RegionCode { get; set; }
    
        public virtual User User { get; set; }
    }
}