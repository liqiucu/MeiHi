using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models.Booking
{
    public class ShopsBookingManageModel
    {
        public int TotalNotPayedCount { get; set; }

        public decimal TotalNotPayedMoney { get; set; }

        public decimal TotalPayedMoney { get; set; }

        public decimal TotalGotedMoney { get; set; }

        public int TotalCancelCount { get; set; }

        public decimal TotalCancelMoney { get; set; }
    }
}