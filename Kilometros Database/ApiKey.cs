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
    
    public partial class ApiKey
    {
        public ApiKey()
        {
            this.Token = new HashSet<Token>();
        }
    
        public System.DateTime CreationDate { get; set; }
        public string Platform { get; set; }
        public string Description { get; set; }
        public System.Guid Guid { get; set; }
        public System.Guid Secret { get; set; }
        public string TokenUpgradeRequired { get; set; }
    
        public virtual ICollection<Token> Token { get; set; }
        public virtual ApiKey ApiKeyPrevious { get; set; }
        public virtual ApiKey ApiKeyNext { get; set; }
    }
}
