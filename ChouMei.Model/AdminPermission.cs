//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChouMei.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class AdminPermission
    {
        public int AdminId { get; set; }
        public int PermissionId { get; set; }
        public bool Denied { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual Admin Admin { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
