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
    
    public partial class Tip
    {
        public Tip()
        {
            this.TipGlobalization = new HashSet<TipGlobalization>();
            this.TipMotionLevel = new HashSet<MotionLevel>();
        }
    
        public System.Guid Guid { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<long> DistanceTrigger { get; set; }
        public long DaysTrigger { get; set; }
        public string Source { get; set; }
    
        public virtual TipCategory TipCategory { get; set; }
        public virtual ICollection<TipGlobalization> TipGlobalization { get; set; }
        public virtual ICollection<MotionLevel> TipMotionLevel { get; set; }
        public virtual UserTipHistory UserTipHistory { get; set; }
    }
}
