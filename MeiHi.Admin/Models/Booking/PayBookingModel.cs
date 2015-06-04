using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models.Booking
{
    public class PayBookingModel
    {
        public long BookingId { get; set; }

        //public string MeiHiTicket { get; set; }

        public decimal Cost { get; set; }

        public string AliAccount { get; set; }

        public string WeiXinAccount { get; set; }
    }
}