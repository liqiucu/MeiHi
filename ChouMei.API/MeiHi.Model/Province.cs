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
    
    public partial class Province
    {
        public Province()
        {
            this.Region = new HashSet<Region>();
        }
    
        public int ProvinceId { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual ICollection<Region> Region { get; set; }
    }
}
