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
    
    public partial class UserDataTotalDistance
    {
        public System.Guid User_Guid { get; set; }
        public DataActivity Activity { get; set; }
        public Nullable<System.DateTime> Timestamp { get; set; }
        public Nullable<long> TotalSteps { get; set; }
        public Nullable<long> TotalDistance { get; set; }
    }
}
