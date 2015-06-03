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


            return View();
        }
    }
}