using MeiHi.Admin.Logic;
using MeiHi.Admin.Models.Booking;
using MeiHi.Admin.Models.Shoper;
using MeiHi.Model;
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
        [Auth(PermissionName = "订单与资金维护管理")]
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

        [Auth(PermissionName = "订单与资金维护管理")]
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

        [Auth(PermissionName = "订单与资金维护管理")]
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

        [Auth(PermissionName = "订单与资金维护管理")]
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

        [Auth(PermissionName = "订单与资金维护管理")]
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
        [Auth(PermissionName = "订单与资金维护管理")]
        public ActionResult PayUnBillingBooking(long bookingId)
        {
            using (var db = new MeiHiEntities())
            {
                PayBookingModel model = new PayBookingModel()
                {
                    BookingId = bookingId
                };

                var booking = db.Booking.FirstOrDefault(a => a.BookingId == bookingId);

                if (booking == null)
                {
                    ModelState.AddModelError("", "无效的订单");
                    return View(model);
                }

                model.Cost = booking.Cost;

                return View(model);
            }
        }
        [Auth(PermissionName = "订单与资金维护管理")]
        /// <summary>
        /// 这个是批量支付 较危险 只能一个人操作
        /// 设置权限的时候只有一个人有权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PayUnBillingBooking(PayBookingModel model)
        {
            if (string.IsNullOrEmpty(model.AliAccount) && string.IsNullOrEmpty(model.WeiXinAccount))
            {
                ModelState.AddModelError("", "账号必须输入一个");
                return View(model);
            }

            using (var db = new MeiHiEntities())
            {
                var booking = db.Booking.FirstOrDefault(a => a.BookingId == model.BookingId);

                if (booking == null)
                {
                    ModelState.AddModelError("", "订单无效");
                    return View(model);
                }

                if (booking.Cost != model.Cost)
                {
                    ModelState.AddModelError("", "金额不匹配 实际金额应该是:" + booking.Cost);
                    return View(model);
                }

                try
                {
                    if (booking != null)
                    {
                        booking.Status = true;
                        db.SaveChanges();

                        if (!string.IsNullOrEmpty(model.AliAccount))
                        {
                            //if pay success
                            //else booking.Status = false; db.SaveChanges();
                        }
                        else
                        {
                            //if pay success
                            //else booking.Status = false; db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    booking.Status = false;
                    db.SaveChanges();
                }

                return RedirectToAction("ManageBookings", new { bookingId = model.BookingId });
            }
        }
    }
}