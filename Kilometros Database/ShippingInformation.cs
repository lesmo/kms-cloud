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
    
    public partial class ShippingInformation
    {
        public long Id { get; set; }
        public string RegionCode { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public System.DateTime LastEditDate { get; set; }
    
        public virtual User User { get; set; }
    }
}
