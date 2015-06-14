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
        [Auth(Roles = "店主")]
        public ActionResult ManageBookings(int page = 1, string meihiTicket = "", long bookingId = 0)
        {
            if (Session["ShopId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = (long)Session["ShopId"];
            ShopsBookingManageModel model = new ShopsBookingManageModel();

            if (!string.IsNullOrEmpty(meihiTicket) && bookingId > 0)
            {
                ModelState.AddModelError("", "一次查询只能输入一个美嗨券或者订单ID");
                return View(model);
            }

            model = BookingLogic.GetAllBookings(shopId, page, 10, meihiTicket, bookingId);
            model.ShopId = shopId;
            return View(model);
        }

        /// <summary>
        /// 已支付订单列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="meihiTicket"></param>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [Auth(Roles = "店主")]
        public ActionResult ManageBookingHaveBillinged(int page = 1, string meihiTicket = "", long bookingId = 0)
        {
            if (Session["ShopId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = (long)Session["ShopId"];

            ShopsBookingManageModel model = new ShopsBookingManageModel();

            if (!string.IsNullOrEmpty(meihiTicket) && bookingId > 0)
            {
                ModelState.AddModelError("", "一次查询只能输入一个美嗨券或者订单ID");
                return View(model);
            }

            model = BookingLogic.GetAllBillingedBookings(shopId, page, 10, meihiTicket, bookingId);

            return View(model);
        }

        /// <summary>
        /// 正在申请退款订单列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="meihiTicket"></param>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [Auth(Roles = "店主")]
        public ActionResult ManageCancelBooking(int page = 1, string meihiTicket = "", long bookingId = 0)
        {
            if (Session["ShopId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = (long)Session["ShopId"];

            ShopsBookingManageModel model = new ShopsBookingManageModel();

            if (!string.IsNullOrEmpty(meihiTicket) && bookingId > 0)
            {
                ModelState.AddModelError("", "一次查询只能输入一个美嗨券或者订单ID");
                return View(model);
            }

            model = BookingLogic.GetAllCancelBookings(shopId, page, 10, meihiTicket, bookingId);

            return View(model);
        }

        /// <summary>
        /// 未结清订单
        /// </summary>
        /// <param name="page"></param>
        /// <param name="meihiTicket"></param>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [Auth(Roles = "店主")]
        public ActionResult ManageUnPayToShopBooking(int page = 1, string meihiTicket = "", long bookingId = 0)
        {
            if (Session["ShopId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = (long)Session["ShopId"];
            ShopsBookingManageModel model = new ShopsBookingManageModel();

            if (!string.IsNullOrEmpty(meihiTicket) && bookingId > 0)
            {
                ModelState.AddModelError("", "一次查询只能输入一个美嗨券或者订单ID");
                return View(model);
            }

            model = BookingLogic.GetAllUnPayToShopBookings(shopId, page, 10, meihiTicket, bookingId);

            return View(model);
        }
    }
}