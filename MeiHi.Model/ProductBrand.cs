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
    
    public partial class ProductBrand
    {
        public long ProductBrandId { get; set; }
        public string ProductUrl { get; set; }
        public string ProductName { get; set; }
        public System.DateTime DateCreated { get; set; }
        public long ShopId { get; set; }
    
        public virtual Shop Shop { get; set; }
    }
}
