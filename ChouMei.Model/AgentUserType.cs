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
    
    public partial class AgentUserType
    {
        public AgentUserType()
        {
            this.AgentUsers = new HashSet<AgentUser>();
            this.AgentUserTypeAgentPermissions = new HashSet<AgentUserTypeAgentPermission>();
        }
    
        public int TypeId { get; set; }
        public string Name { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual ICollection<AgentUser> AgentUsers { get; set; }
        public virtual ICollection<AgentUserTypeAgentPermission> AgentUserTypeAgentPermissions { get; set; }
    }
}
