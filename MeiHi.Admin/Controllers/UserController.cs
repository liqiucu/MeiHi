﻿using MeiHi.Admin.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;
using MeiHi.Admin.Logic;
using MeiHi.Admin.Models.UserComments;

namespace MeiHi.Admin.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult ManageUsers(int page = 1, string userName = "")
        {
            var model = new UsersModel();
            model.Users = UserLogic.GetUsers(page, 10, userName);
            return View(model);
        }

        public ActionResult ManageUserCommentsByUserId(long userId, int page = 1)
        {
            return View(UserLogic.GetUserCommentsByUserId(userId, page, 10));
        }

        public ActionResult ManageUserBookingsByUserId(long userId, int page = 1)
        {
            var model = new UserBookingsModel();
            model.UserBookings = UserLogic.GetUserBookingsByUserId(userId, page, 10);

            return View(model);
        }

        public ActionResult RefundToUserByBookingId(long userBookingId)
        {
            using (var db = new MeiHiEntities())
            {
                var booking = db.Booking.First(a => a.BookingId == userBookingId);

                if (booking.IsBilling
                    && !booking.IsUsed
                    && !booking.Status
                    && booking.Cancel
                    && !booking.CancelSuccess)
                {
                    //if pay success
                    booking.CancelSuccess = true;
                    db.SaveChanges();
                }

                return RedirectToAction("ManageUserBookingsByUserId", new { userId = booking.UserId });
            }
        }

        public ActionResult UnDisplayUserComment(long userCommentId, bool allManage = false)
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
                if (allManage)
                {
                    return RedirectToAction("ManageAllComments", new { userCommentId = userCommentId });
                }

                return RedirectToAction("ManageUserCommentsByUserId", new { userId = userComment.UserId });
            }
        }

        public ActionResult DisplayUserComment(long userCommentId, bool allManage = false)
        {
            using (var db = new MeiHiEntities())
            {
                var userComment = db.UserComments.FirstOrDefault(a => a.UserCommentId == userCommentId);

                if (userComment != null)
                {
                    userComment.Display = true;
                    userComment.DateModified = DateTime.Now;
                    db.SaveChanges();
                }
                if (allManage)
                {
                    return RedirectToAction("ManageAllComments", new { userCommentId = userCommentId });
                }

                return RedirectToAction("ManageUserCommentsByUserId", new { shopId = userComment.ShopId });
            }
        }

        [HttpPost]
        public ActionResult ReplyUserComment(UserCommentsReplyModel model, bool allManage = false)
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
                if (allManage)
                {
                    return RedirectToAction("ManageAllComments", new { userCommentId = model.UserCommentId });
                }
                return RedirectToAction("ManageUserCommentsByUserId", new { userId = model.UserId });
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
                    model.UserId = userComment.UserId;
                }

                return View(model);
            }
        }

        public ActionResult ManageAllComments(
            int page = 1,
            string userName = "",
            string mobile = "",
            string content = "",
            string shopName = "",
            string serviceName = "",
            long userCommentId = 0,
            bool lowerRate = false,
            bool higherRate = false)
        {
            if (lowerRate)
            {
                ViewData["LowerRate"] = lowerRate;
                var model = UserLogic.GetAllLowerRateUserComments(page, 10);
                return View(model);
            }

            if (higherRate)
            {

                ViewData["HigherRate"] = higherRate;
                var model = UserLogic.GetAllHighterRateUserComments(page, 10);
                return View(model);
            }

            if (!string.IsNullOrEmpty(userName)
                || !string.IsNullOrEmpty(mobile)
                || !string.IsNullOrEmpty(content)
                || !string.IsNullOrEmpty(shopName)
                || !string.IsNullOrEmpty(serviceName)
                )
            {
                ViewData["UserName"] = userName;
                ViewData["Mobile"] = mobile;
                ViewData["Content"] = content;
                ViewData["ShopName"] = shopName;
                ViewData["ServiceName"] = serviceName;

                var model = UserLogic.GetAllUserComments(
                                page,
                                10,
                                userName,
                                mobile,
                                content,
                                shopName,
                                serviceName,
                                userCommentId);
                return View(model);
            }
            else
            {
                var model = UserLogic.GetAllUserComments(page, 10);
                return View(model);
            }
        }
    }
}