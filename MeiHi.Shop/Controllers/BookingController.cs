using MeiHi.Shop.Logic;
using MeiHi.Shop.Models.Booking;
using MeiHi.Shop.Models.Shoper;
using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeiHi.Shop.Controllers
{
    public class BookingController : Controller
    {
        // GET: Booking
        public ActionResult ManageBookings(long shoppId, int page = 1, string meihiTicket = "", long bookingId = 0)
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

        /// <summary>
        /// 已支付订单列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="meihiTicket"></param>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        public ActionResult ManageBookingHaveBillinged(int page = 1, string meihiTicket = "", long bookingId = 0)
        {
            ShopsBookingManageModel model = new ShopsBookingManageModel();

            if (!string.IsNullOrEmpty(meihiTicket) && bookingId > 0)
            {
                ModelState.AddModelError("", "一次查询只能输入一个美嗨券或者订单ID");
                return View(model);
            }

            model = BookingLogic.GetAllBillingedBookings(page, 10, meihiTicket, bookingId);

            return View(model);
        }

        /// <summary>
        /// 正在申请退款订单列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="meihiTicket"></param>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        public ActionResult ManageCancelBooking(int page = 1, string meihiTicket = "", long bookingId = 0)
        {
            ShopsBookingManageModel model = new ShopsBookingManageModel();

            if (!string.IsNullOrEmpty(meihiTicket) && bookingId > 0)
            {
                ModelState.AddModelError("", "一次查询只能输入一个美嗨券或者订单ID");
                return View(model);
            }

            model = BookingLogic.GetAllCancelBookings(page, 10, meihiTicket, bookingId);

            return View(model);
        }

        /// <summary>
        /// 未结清订单
        /// </summary>
        /// <param name="page"></param>
        /// <param name="meihiTicket"></param>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        public ActionResult ManageUnPayToShopBooking(int page = 1, string meihiTicket = "", long bookingId = 0)
        {
            ShopsBookingManageModel model = new ShopsBookingManageModel();

            if (!string.IsNullOrEmpty(meihiTicket) && bookingId > 0)
            {
                ModelState.AddModelError("", "一次查询只能输入一个美嗨券或者订单ID");
                return View(model);
            }

            model = BookingLogic.GetAllUnPayToShopBookings(page, 10, meihiTicket, bookingId);

            return View(model);
        }

        public ActionResult RefundToUserByBookingId(long bookingId)
        {
            using (var db = new MeiHiEntities())
            {
                var booking = db.Booking.First(a => a.BookingId == bookingId
                    && a.IsBilling
                    && !a.IsUsed
                    && !a.Status
                    && a.Cancel
                    && !a.CancelSuccess);

                try
                {
                    if (booking != null)
                    {
                        booking.CancelSuccess = true;
                        db.SaveChanges();
                        //if pay success
                        //else booking.CancelSuccess = false; db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    booking.CancelSuccess = false;
                    db.SaveChanges();
                }

                return RedirectToAction("ManageBookings", new { bookingId = bookingId });
            }
        }
    }
}