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
    
    public partial class MarketerRegister
    {
        public long RegisterUserId { get; set; }
        public int MarketerId { get; set; }
        public string Mobile { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime RegisterDate { get; set; }
    
        public virtual Marketer Marketer { get; set; }
    }
}
