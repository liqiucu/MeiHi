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
    
    public partial class HairStyle
    {
        public long HairStyleId { get; set; }
        public string HairStyleUrl { get; set; }
        public string HairStyleModelUrl { get; set; }
        public long HairStyleTypeId { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        public virtual HairStyleType HairStyleType { get; set; }
    }
}