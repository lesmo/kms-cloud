//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KilometrosDatabase
{
    using System;
    using System.Collections.Generic;
    
    public partial class Notification
    {
        public System.Guid Guid { get; set; }
        public string CreationDate { get; set; }
        public System.Guid ObjectGuid { get; set; }
        public NotificationType NotificationType { get; set; }
        public bool Discarded { get; set; }
    
        public virtual User User { get; set; }
    }
}
