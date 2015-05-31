using MeiHi.Admin.Models;
using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PagedList.Mvc;
using PagedList;
using Owin;
using MeiHi.Admin.Logic;
using System.Data.Entity.Validation;
using MeiHi.Admin.Models.Service;
using System.IO;
using MeiHi.CommonDll.Helper;
using MeiHi.Admin.Models.Shoper;
using MeiHi.Admin.Models.Booking;
using MeiHi.Admin.Models.User;

namespace MeiHi.Admin.Controllers
{
    public class ShoperController : Controller
    {
        // GET: Shoper
        public ActionResult ManageShopers(int page = 1)
        {
            var model = new ShopersModel()
            {
                Shopers = ShoperLogic.GetShopers(page, 10)
            };

            return View(model);
        }

        public ActionResult GetUnBillingBookingsByShopId(long shopId, int page = 1)
        {
            return View(ShoperLogic.GetUnBillingBookingsByShopId(shopId, page, 10));
        }

        [HttpPost]
        public ActionResult PayAllUnBillingBookings(PayAllUnBillingBookingsModel model)
        {
            //if (!string.IsNullOrEmpty(model.AliAccount) && !string.IsNullOrEmpty(model.WeiXinAccount))
            //{
            //    TempData["PayAllUnBillingBookingsError"] = "账号只能输入一个";
            //    return RedirectToAction("GetUnBillingBookingsByShopId", new { shopId = model.ShopId });
            //}

            if (string.IsNullOrEmpty(model.AliAccount) && string.IsNullOrEmpty(model.WeiXinAccount))
            {
                TempData["PayAllUnBillingBookingsError"] = "账号必须输入一个";
                return RedirectToAction("GetUnBillingBookingsByShopId", new { shopId = model.ShopId });
            }

            using (var db = new MeiHiEntities())
            {
                var bookings = db.Booking.Where(a => a.ShopId == model.ShopId && a.IsBilling && a.IsUsed && !a.Status);

                var temp = bookings != null && bookings.Count() > 0 ? bookings.Sum(a => a.Cost) : 0;

                if (temp != model.Cost)
                {
                    TempData["PayAllUnBillingBookingsError"] = "金额不匹配 实际金额应该是:" + temp;
                    return RedirectToAction("GetUnBillingBookingsByShopId", new { shopId = model.ShopId });
                }

                if (!string.IsNullOrEmpty(model.AliAccount))
                {
                    //call ali api success
                }
                else
                {
                    //call ali api success
                }

                foreach (var item in bookings)
                {
                    item.Status = true;
                }

                db.SaveChanges();
            }

            return RedirectToAction("GetUnBillingBookingsByShopId", new { shopId = model.ShopId });
        }

        public ActionResult GetBillingedBookingsByShopId(long shopId, int page = 1)
        {
            return View(ShoperLogic.GetBillingedBookingsByShopId(shopId, page, 10));
        }

        public ActionResult ManageUserCommentsByShopId(long shopId, int page = 1)
        {
            return View(ShoperLogic.GetUserCommentsByShopId(shopId, page, 10));
        }

        public ActionResult UnDisplayUserComment(long userCommentId)
        {
            using (var db = new MeiHiEntities())
            {
                var userComment = db.UserComments.FirstOrDefault(a => a.UserCommentId == userCommentId);

                if (userComment != null)
                {
                    userComment.Display = false;
                    userComment.DateModified = DateTime.Now;
                    db.SaveChanges();
                }

                return RedirectToAction("ManageUserCommentsByShopId", new { shopId = userComment.ShopId });
            }
        }

        [HttpPost]
        public ActionResult ReplyUserComment(UserCommentsReplyModel model)
        {
            using (var db = new MeiHiEntities())
            {
                db.UserCommentsReply.Add(new UserCommentsReply()
                {
                    Comment = model.MeiHiContent,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    UserCommentId = model.UserCommentId
                });

                db.SaveChanges();
                return RedirectToAction("ManageUserCommentsByShopId", new { shopId = model.ShopId });
            }
        }

        public ActionResult ReplyUserComment(long userCommentId)
        {
            using (var db = new MeiHiEntities())
            {
                UserCommentsReplyModel model = new UserCommentsReplyModel();
                var userComment = db.UserComments.FirstOrDefault(a => a.UserCommentId == userCommentId);

                if (userComment != null)
                {
                    model.UserCommentId = userCommentId;
                    model.UserContent = userComment.Comment;
                    model.UserName = userComment.User.FullName;
                    model.ShopId = userComment.ShopId;
                }

                return View(model);
            }
        }
    }
}