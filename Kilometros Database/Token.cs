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
    
    public partial class Token
    {
        public System.DateTimeOffset CreationDate { get; set; }
        public Nullable<System.DateTimeOffset> ExpirationDate { get; set; }
        public System.Guid Guid { get; set; }
        public Nullable<System.DateTime> LastUseDate { get; set; }
    
        public virtual ApiKey ApiKey { get; set; }
        public virtual User User { get; set; }
    }
}
