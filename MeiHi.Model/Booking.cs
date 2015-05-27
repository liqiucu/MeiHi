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
    
    public partial class Booking
    {
        public long BookingId { get; set; }
        public long UserId { get; set; }
        public string Mobile { get; set; }
        public int Count { get; set; }
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public long ShopId { get; set; }
        public string ShopName { get; set; }
        public string Designer { get; set; }
        public decimal Cost { get; set; }
        public bool Status { get; set; }
        public bool IsUsed { get; set; }
        public bool IsBilling { get; set; }
        public bool Cancel { get; set; }
        public bool CancelSuccess { get; set; }
        public string AlipayAccount { get; set; }
        public string WeiXinAccount { get; set; }
        public string VerifyCode { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
    
        public virtual Service Service { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual User User { get; set; }
    }
}
