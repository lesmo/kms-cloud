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
    
    public partial class WebAutoLoginToken
    {
        public long Id { get; set; }
        public System.DateTime CreationDate { get; set; }
        public long Key { get; set; }
        public System.Guid Secret { get; set; }
        public string IPAddress { get; set; }
    
        public virtual Token Token { get; set; }
    }
}
