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
    
    public partial class Region
    {
        public Region()
        {
            this.Shop = new HashSet<Shop>();
            this.Shop1 = new HashSet<Shop>();
        }
    
        public int RegionId { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentRegionId { get; set; }
        public Nullable<int> RootRegionId { get; set; }
        public Nullable<int> ProvinceId { get; set; }
        public bool RequireVerify { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
    
        public virtual Province Province { get; set; }
        public virtual ICollection<Shop> Shop { get; set; }
        public virtual ICollection<Shop> Shop1 { get; set; }
    }
}
