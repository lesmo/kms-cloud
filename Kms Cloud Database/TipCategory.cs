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
    
    public partial class TipCategory : IPicture
    {
        public TipCategory()
        {
            this.TipCategoryGlobalization = new HashSet<TipCategoryGlobalization>();
            this.Tip = new HashSet<Tip>();
        }
    
    
        public virtual ICollection<TipCategoryGlobalization> TipCategoryGlobalization { get; set; }
        public virtual ICollection<Tip> Tip { get; set; }
    }
}
