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
    
    public partial class Admin
    {
        public Admin()
        {
            this.AdminLogons = new HashSet<AdminLogon>();
            this.AdminPermissions = new HashSet<AdminPermission>();
            this.AdminRoles = new HashSet<AdminRole>();
            this.NoticeBoards = new HashSet<NoticeBoard>();
        }
    
        public int AdminId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public Nullable<System.DateTime> LastLogonDate { get; set; }
        public Nullable<System.DateTime> DateDeleted { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
    
        public virtual ICollection<AdminLogon> AdminLogons { get; set; }
        public virtual ICollection<AdminPermission> AdminPermissions { get; set; }
        public virtual ICollection<AdminRole> AdminRoles { get; set; }
        public virtual ICollection<NoticeBoard> NoticeBoards { get; set; }
    }
}
