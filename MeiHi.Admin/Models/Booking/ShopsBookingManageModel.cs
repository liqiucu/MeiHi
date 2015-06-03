using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models.Booking
{
    public class ShopsBookingManageModel
    {
        [Display(Name = "未结清订单数")]
        public int TotalNotPayedCount { get; set; }

        [Display(Name = "未结清金额")]
        public decimal TotalNotPayedMoney { get; set; }

        [Display(Name = "已结清金额")]
        public decimal TotalPayedMoney { get; set; }

        [Display(Name = "销售总额")]
        public decimal TotalGotedMoney { get; set; }

        [Display(Name = "当前正在退单数")]
        public int TotalCancelCount { get; set; }

        [Display(Name = "当前正在退单金额")]
        public decimal TotalCancelMoney { get; set; }

        public StaticPagedList<BookingModel> UserBookings { get; set; }
    }
}