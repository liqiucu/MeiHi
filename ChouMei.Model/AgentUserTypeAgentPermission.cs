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
    
    public partial class AgentUserTypeAgentPermission
    {
        public int TypeId { get; set; }
        public int PermissionId { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual AgentPermission AgentPermission { get; set; }
        public virtual AgentUserType AgentUserType { get; set; }
    }
}
