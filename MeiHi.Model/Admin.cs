//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeiHi.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Admin
    {
        public Admin()
        {
            this.AdminPermission = new HashSet<AdminPermission>();
            this.AdminRole = new HashSet<AdminRole>();
        }
    
        public int AdminId { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public bool Avaliable { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
    
        public virtual ICollection<AdminPermission> AdminPermission { get; set; }
        public virtual ICollection<AdminRole> AdminRole { get; set; }
    }
}
