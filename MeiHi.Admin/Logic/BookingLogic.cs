using MeiHi.Admin.Models.Booking;
using MeiHi.Model;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Logic
{
    public class BookingLogic
    {
        public static ShopsBookingManageModel GetAllBookings(int page, int pageSize)
        {
            using (var db = new MeiHiEntities())
            {
                var userBookings = db.Booking.OrderByDescending(a => a.IsBilling).OrderByDescending(a => a.DateCreated).Skip((page - 1) * pageSize).Take(pageSize);
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

                ShopsBookingManageModel result = new ShopsBookingManageModel();

                result.TotalCancelCount = db.Booking.Where(a => a.IsBilling && !a.IsUsed && a.Cancel && !a.CancelSuccess).Count();
                result.TotalCancelMoney = db.Booking.Where(a => a.IsBilling && !a.IsUsed && a.Cancel && !a.CancelSuccess).Sum(a => a.Cost);
                result.TotalNotPayedCount = db.Booking.Where(a => a.IsBilling && a.IsUsed && !a.Cancel && !a.Status).Count();
                result.TotalNotPayedMoney = db.Booking.Where(a => a.IsBilling && a.IsUsed && !a.Cancel && !a.Status).Sum(a=>a.Cost);
                result.TotalGotedMoney = db.Booking.Where(a => a.IsBilling && a.IsUsed).Sum(a => a.Cost);
                result.TotalPayedMoney = db.Booking.Where(a => a.IsBilling && a.IsUsed && a.Status).Sum(a => a.Cost);

                result.UserBookings = new StaticPagedList<BookingModel>(bookings, page, pageSize, db.Booking.Count());
                return result;
            }
        }
    }
}