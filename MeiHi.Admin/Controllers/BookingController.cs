using MeiHi.Admin.Logic;
using MeiHi.Admin.Models.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeiHi.Admin.Controllers
{
    public class BookingController : Controller
    {
        // GET: Booking
        public ActionResult ManageBookings(int page = 1, string meihiTicket = "", long bookingId = 0)
        {
            ShopsBookingManageModel model = new ShopsBookingManageModel();

            if (!string.IsNullOrEmpty(meihiTicket) && bookingId > 0)
            {
                ModelState.AddModelError("", "一次查询只能输入一个美嗨券或者订单ID");
                return View(model);
            }

            model = BookingLogic.GetAllBookings(page, 10, meihiTicket, bookingId);

            return View(model);
        }
    }
}