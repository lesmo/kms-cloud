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
    
    public partial class UserDataHourlyDistance
    {
        public DataActivity Activity { get; set; }
        public System.DateTime Timestamp { get; set; }
        public long Steps { get; set; }
        public long Distance { get; set; }
    
        public virtual User User { get; set; }
    }
}
