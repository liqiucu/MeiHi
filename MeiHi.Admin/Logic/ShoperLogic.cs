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
using MeiHi.Admin.Models.Shoper;

namespace MeiHi.Admin.Logic
{
    public static class ShoperLogic
    {
        public static StaticPagedList<ShoperModel> GetShopers(int page, int pageSize, string shopName="")
        {
            using (var db = new MeiHiEntities())
            {
                var shopers = db.Shop.OrderByDescending(a => a.Booking.Where(c => !c.Status && c.IsBilling).Sum(b => b.Cost)).Skip((page - 1) * pageSize).Take(pageSize);

                if (!string.IsNullOrEmpty(shopName))
                {
                    shopers = db.Shop.Where(a => a.Title.Contains("shopName")).OrderByDescending(a => a.Booking.Where(c => !c.Status && c.IsBilling).Sum(b => b.Cost)).Skip((page - 1) * pageSize).Take(pageSize);
                }

                var shopersList = new List<ShoperModel>();

                foreach (var item in shopers)
                {
                    var t1 = item.Booking.Where(a => a.IsBilling && a.IsUsed && !a.Status);

                    var temp = new ShoperModel()
                    {
                        UnBillingedCount = t1.Count(),
                        SumUnBillinged = t1 != null ? t1.Sum(a => a.Cost) : 0,
                        ShopComment = item.Comment,
                        ShoperId = item.ShopUser.First().ShopUserId,
                        ShopId = item.ShopId,
                        ShopName = item.Title,
                        UserName = item.Phone,
                        FullName = item.ShopUser.First().FullName,
                        AliPayAccount = item.ShopUser.First().AliPayAccount,
                        WinXinPayAccount = item.ShopUser.First().WeiXinPayAccount
                    };

                    shopersList.Add(temp);
                }

                return new StaticPagedList<ShoperModel>(shopersList, page, pageSize, db.Shop.Count());
            }
        }

        public static UserBookingsModel GetUnBillingBookingsByShopId(long shopId, int page, int pageSize)
        {
            using (var db = new MeiHiEntities())
            {
                var t3 = db.Booking.Where(a => a.ShopId == shopId && a.IsBilling && a.IsUsed && !a.Status);
                var userBookings = t3.OrderByDescending(a => a.ShopId).Skip((page - 1) * pageSize).Take(pageSize);
                var shopUser = db.ShopUser.FirstOrDefault(a => a.ShopId == shopId);
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
                UserBookingsModel result = new UserBookingsModel();
                result.UserBookings = new StaticPagedList<BookingModel>(bookings, page, pageSize, t3.Count());
                result.AliAccount = shopUser.AliPayAccount;
                result.WeiXinAccount = shopUser.WeiXinPayAccount;
                result.ShopId = shopId;
                result.TotalCost =t3!=null&&t3.Count()>0? t3.Sum(a => a.Cost):0;
                result.TotalCount = t3.Count();
                return result;
            }
        }

        public static UserBookingsModel GetBillingedBookingsByShopId(long shopId, int page, int pageSize)
        {
            using (var db = new MeiHiEntities())
            {
                var t3 = db.Booking.Where(a => a.ShopId == shopId && a.IsBilling && a.IsUsed && a.Status);
                var userBookings = t3.OrderByDescending(a => a.ShopId).Skip((page - 1) * pageSize).Take(pageSize);
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

                UserBookingsModel result = new UserBookingsModel();
                result.UserBookings = new StaticPagedList<BookingModel>(bookings, page, pageSize, t3.Count());
                result.TotalCost = t3 != null && t3.Count() > 0 ? t3.Sum(a => a.Cost) : 0;
                result.TotalCount = t3.Count();
                return result;
            }
        }

        public static UserCommentsListModel GetUserCommentsByShopId(long shopId, int page, int pageSize)
        {
            using (var db = new MeiHiEntities())
            {
                UserCommentsListModel result=new UserCommentsListModel();

                var userComments = db.UserComments.Where(a => a.ShopId == shopId)
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
                            UserId = shopId,
                            Mobile = item.User.Mobile,
                            ShopName = item.Shop.Title,
                            Display = item.Display.Value,
                            UserSharedImgaeList = temp != null && temp.Count() > 0 ? temp.Select(a => a.ImgUrl).ToList() : null,
                            MeiHiReply = temp1 != null && temp1.Count() > 0 ? temp1.Select(a => a.Comment).ToList() : null,
                        });
                    }
                }

                result.UserCommentsList=new StaticPagedList<UserCommentsModel>(comments, page, pageSize, userComments.Count());
                result.ShopId=shopId;
                result.ShopName=db.Shop.FirstOrDefault(a=>a.ShopId==shopId).Title;

                return result;
            }
        }
    }
}