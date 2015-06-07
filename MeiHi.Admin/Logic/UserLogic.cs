using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using MeiHi.Admin.Models;
using PagedList;
using MeiHi.Admin.Models.UserComments;
using MeiHi.Admin.Models.User;
using MeiHi.Admin.Models.Booking;

namespace MeiHi.Admin.Logic
{
    public static class UserLogic
    {
        public static StaticPagedList<UserModel> GetUsers(int page, int pageSize, string userName = "")
        {
            using (var access = new MeiHiEntities())
            {
                var users = access.User.OrderByDescending(a => a.Booking.Any(b => b.Cancel && b.CancelSuccess == false)).Skip((page - 1) * pageSize).Take(pageSize);

                if (!string.IsNullOrEmpty(userName))
                {
                    users = access.User.Where(a => a.FullName.Contains(userName)).OrderByDescending(a => a.Booking.Any(b => b.Cancel && b.CancelSuccess == false)).Skip((page - 1) * pageSize).Take(pageSize);
                }

                var usersList = new List<UserModel>();

                foreach (var item in users)
                {
                    var t1 = item.Booking.FirstOrDefault();
                    var bookings = item.Booking.Where(a => a.IsBilling);

                    var temp = new UserModel()
                    {
                        AliPayAccount = t1 != null ? t1.AlipayAccount : "",
                        Blance = item.Balance,
                        Mobile = item.Mobile,
                        UserId = item.UserId,
                        UserName = item.FullName,
                        WeiXinPayAccount = t1 != null ? t1.WeiXinAccount : "",
                        HaveBillingCount = item.Booking.Count(a => a.IsBilling),
                        HaveBillingMoney = bookings != null ? bookings.Sum(a => a.Cost) : 0,
                        HaveCancelBooking = item.Booking.Any(a => a.Cancel && a.CancelSuccess == false)
                    };

                    usersList.Add(temp);
                }

                return new StaticPagedList<UserModel>(usersList, page, pageSize, access.User.Count());
            }
        }

        public static UserCommentsListModel GetUserCommentsByUserId(long userId, int page, int pageSize)
        {
            using (var db = new MeiHiEntities())
            {
                UserCommentsListModel result = new UserCommentsListModel();
                var userComments = db.UserComments.Where(a => a.UserId == userId)
                                    .OrderByDescending(a => a.Display).Skip((page - 1) * pageSize).Take(pageSize);
                var comments = new List<UserCommentsModel>();

                if (userComments != null && userComments.Count() > 0)
                {
                    foreach (var item in userComments)
                    {
                        var temp = item.UserCommentSharedImg.Where(a => a.UserCommentId == item.UserCommentId);
                        var temp1 = item.UserCommentsReply.Where(a => a.UserCommentId == item.UserCommentId);

                        comments.Add(new UserCommentsModel()
                        {
                            Comment = item.Comment,
                            DateCreated = item.DateCreated,
                            Rate = item.Rate,
                            ServiceName = item.ServiceName,
                            ShopId = item.ShopId,
                            UserFullName = item.User.FullName,
                            UserCommentId = item.UserCommentId,
                            UserId = userId,
                            Mobile = item.User.Mobile,
                            ShopName = item.Shop.Title,
                            Display = item.Display.Value,
                            UserSharedImgaeList = temp != null && temp.Count() > 0 ? temp.Select(a => a.ImgUrl).ToList() : null,
                            MeiHiReply = temp1 != null && temp1.Count() > 0 ? temp1.Select(a => a.Comment).ToList() : null,
                        });
                    }
                }
                result.UserCommentsList = new StaticPagedList<UserCommentsModel>(comments, page, pageSize, userComments.Count());
                result.UserFullName = db.User.FirstOrDefault(a => a.UserId == userId).FullName;
                result.UserId = userId;
                return result;
            }
        }

        public static StaticPagedList<BookingModel> GetUserBookingsByUserId(long userId, int page, int pageSize)
        {
            using (var db = new MeiHiEntities())
            {
                var userBookings = db.Booking.Where(a => a.UserId == userId)
                                    .OrderByDescending(a => a.Cancel).Skip((page - 1) * pageSize).Take(pageSize);
                var bookings = new List<BookingModel>();

                if (userBookings != null && userBookings.Count() > 0)
                {
                    foreach (var item in userBookings)
                    {
                        bookings.Add(new BookingModel()
                        {
                            AlipayAccount = item.AlipayAccount,
                            WeiXinAccount = item.WeiXinAccount,
                            Designer = item.Designer,
                            ServiceId = item.ServiceId,
                            BookingId = item.BookingId,
                            Cancel = item.Cancel,
                            CancelSuccess = item.CancelSuccess,
                            Cost = item.Cost,
                            Count = item.Count,
                            DateCreated = item.DateCreated,
                            DateModified = item.DateModified,
                            Mobile = item.Mobile,
                            IsBilling = item.IsBilling,
                            IsUsed = item.IsUsed,
                            ServiceName = item.ServiceName,
                            ShopId = item.ShopId,
                            ShopName = item.ShopName,
                            Status = item.Status,
                            UserId = item.UserId,
                            VerifyCode = item.VerifyCode
                        });
                    }
                }

                return new StaticPagedList<BookingModel>(bookings, page, pageSize, userBookings.Count());
            }
        }

        public static AllUserCommentsModel GetAllUserComments(int page, int pageSize)
        {
            using (var db = new MeiHiEntities())
            {
                var result = new AllUserCommentsModel();
                var userComments = db.UserComments.OrderByDescending(a => a.Display).Skip((page - 1) * pageSize).Take(pageSize);
                var comments = new List<UserCommentsModel>();

                if (userComments != null && userComments.Count() > 0)
                {
                    foreach (var item in userComments)
                    {
                        var temp = item.UserCommentSharedImg.Where(a => a.UserCommentId == item.UserCommentId);
                        var temp1 = item.UserCommentsReply.Where(a => a.UserCommentId == item.UserCommentId);

                        comments.Add(new UserCommentsModel()
                        {
                            Comment = item.Comment,
                            DateCreated = item.DateCreated,
                            Rate = item.Rate,
                            ServiceName = item.ServiceName,
                            ShopId = item.ShopId,
                            UserFullName = item.User.FullName,
                            UserCommentId = item.UserCommentId,
                            UserId = item.UserId,
                            Mobile = item.User.Mobile,
                            ShopName = item.Shop.Title,
                            Display = item.Display.Value,
                            UserSharedImgaeList = temp != null && temp.Count() > 0 ? temp.Select(a => a.ImgUrl).ToList() : null,
                            MeiHiReply = temp1 != null && temp1.Count() > 0 ? temp1.Select(a => a.Comment).ToList() : null,
                        });
                    }
                }
                result.UserCommentsList = new StaticPagedList<UserCommentsModel>(comments, page, pageSize, userComments.Count());
                result.HigherRateCount = db.UserComments.Count(a => a.Rate == 5);
                result.LowerRateCount = db.UserComments.Count(a => a.Rate <= 2);
                return result;
            }
        }

        public static AllUserCommentsModel GetAllLowerRateUserComments(int page, int pageSize)
        {
            using (var db = new MeiHiEntities())
            {
                var result = new AllUserCommentsModel();
                var userComments = db.UserComments.Where(a=>a.Rate<=2).OrderByDescending(a => a.Display).Skip((page - 1) * pageSize).Take(pageSize);
                var comments = new List<UserCommentsModel>();

                if (userComments != null && userComments.Count() > 0)
                {
                    foreach (var item in userComments)
                    {
                        var temp = item.UserCommentSharedImg.Where(a => a.UserCommentId == item.UserCommentId);
                        var temp1 = item.UserCommentsReply.Where(a => a.UserCommentId == item.UserCommentId);

                        comments.Add(new UserCommentsModel()
                        {
                            Comment = item.Comment,
                            DateCreated = item.DateCreated,
                            Rate = item.Rate,
                            ServiceName = item.ServiceName,
                            ShopId = item.ShopId,
                            UserFullName = item.User.FullName,
                            UserCommentId = item.UserCommentId,
                            UserId = item.UserId,
                            Mobile = item.User.Mobile,
                            ShopName = item.Shop.Title,
                            Display = item.Display.Value,
                            UserSharedImgaeList = temp != null && temp.Count() > 0 ? temp.Select(a => a.ImgUrl).ToList() : null,
                            MeiHiReply = temp1 != null && temp1.Count() > 0 ? temp1.Select(a => a.Comment).ToList() : null,
                        });
                    }
                }
                result.UserCommentsList = new StaticPagedList<UserCommentsModel>(comments, page, pageSize, userComments.Count());
                result.HigherRateCount = db.UserComments.Count(a =>a.Rate == 5);
                result.LowerRateCount = db.UserComments.Count(a => a.Rate <= 2);
                return result;
            }
        }

        public static AllUserCommentsModel GetAllHighterRateUserComments(int page, int pageSize)
        {
            using (var db = new MeiHiEntities())
            {
                var result = new AllUserCommentsModel();
                var userComments = db.UserComments.Where(a => a.Rate ==5).OrderByDescending(a => a.Display).Skip((page - 1) * pageSize).Take(pageSize);
                var comments = new List<UserCommentsModel>();

                if (userComments != null && userComments.Count() > 0)
                {
                    foreach (var item in userComments)
                    {
                        var temp = item.UserCommentSharedImg.Where(a => a.UserCommentId == item.UserCommentId);
                        var temp1 = item.UserCommentsReply.Where(a => a.UserCommentId == item.UserCommentId);

                        comments.Add(new UserCommentsModel()
                        {
                            Comment = item.Comment,
                            DateCreated = item.DateCreated,
                            Rate = item.Rate,
                            ServiceName = item.ServiceName,
                            ShopId = item.ShopId,
                            UserFullName = item.User.FullName,
                            UserCommentId = item.UserCommentId,
                            UserId = item.UserId,
                            Mobile = item.User.Mobile,
                            ShopName = item.Shop.Title,
                            Display = item.Display.Value,
                            UserSharedImgaeList = temp != null && temp.Count() > 0 ? temp.Select(a => a.ImgUrl).ToList() : null,
                            MeiHiReply = temp1 != null && temp1.Count() > 0 ? temp1.Select(a => a.Comment).ToList() : null,
                        });
                    }
                }
                result.UserCommentsList = new StaticPagedList<UserCommentsModel>(comments, page, pageSize, userComments.Count());
                result.HigherRateCount = db.UserComments.Count(a => a.Rate == 5);
                result.LowerRateCount = db.UserComments.Count(a => a.Rate <= 2);
                return result;
            }
        }

        public static AllUserCommentsModel GetAllUserComments(
            int page,
            int pageSize,
            string userName = "",
            string mobile = "",
            string content = "",
            string shopName = "",
            string serviceName = "",
            long userCommentId = 0)
        {
            using (var db = new MeiHiEntities())
            {
                var result = new AllUserCommentsModel();
                IQueryable<UserComments> userComments = null;

                if (userCommentId > 0)
                {
                    userComments = db.UserComments.Where(a => a.UserCommentId == userCommentId).OrderByDescending(a => a.Display).Skip((page - 1) * pageSize).Take(pageSize);
                }
                else
                {
                    userComments = db.UserComments.Where
                     (a => a.User.FullName.Contains(userName)
                      && a.User.Mobile.Contains(mobile)
                      && a.Comment.Contains(content)
                      && a.Shop.Title.Contains(shopName)
                      && a.ServiceName.Contains(serviceName)).OrderByDescending(a => a.Display).Skip((page - 1) * pageSize).Take(pageSize);
                }

                var comments = new List<UserCommentsModel>();

                if (userComments != null && userComments.Count() > 0)
                {
                    foreach (var item in userComments)
                    {
                        var temp = item.UserCommentSharedImg.Where(a => a.UserCommentId == item.UserCommentId);
                        var temp1 = item.UserCommentsReply.Where(a => a.UserCommentId == item.UserCommentId);

                        comments.Add(new UserCommentsModel()
                        {
                            Comment = item.Comment,
                            DateCreated = item.DateCreated,
                            Rate = item.Rate,
                            ServiceName = item.ServiceName,
                            ShopId = item.ShopId,
                            UserFullName = item.User.FullName,
                            UserCommentId = item.UserCommentId,
                            UserId = item.UserId,
                            Mobile = item.User.Mobile,
                            ShopName = item.Shop.Title,
                            Display = item.Display.Value,
                            UserSharedImgaeList = temp != null && temp.Count() > 0 ? temp.Select(a => a.ImgUrl).ToList() : null,
                            MeiHiReply = temp1 != null && temp1.Count() > 0 ? temp1.Select(a => a.Comment).ToList() : null,
                        });
                    }
                }
                result.UserCommentsList = new StaticPagedList<UserCommentsModel>(comments, page, pageSize, userComments.Count());
                result.HigherRateCount = db.UserComments.Count(a => a.Rate == 5);
                result.LowerRateCount = db.UserComments.Count(a => a.Rate <= 2);
                return result;
            }
        }
    }
}