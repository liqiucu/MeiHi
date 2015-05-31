using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models.Booking
{
    public class BookingModel
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
    }
}