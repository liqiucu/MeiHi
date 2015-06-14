using MeiHi.API.Helper;
using MeiHi.API.Logic;
using MeiHi.API.ViewModels;
using MeiHi.CommonDll.Helper;
using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MeiHi.API.Controllers
{
    [RoutePrefix("booking")]
    public class BookingController : ApiController
    {
        /// <summary>
        /// 0是阿里 1是微信
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [ApiAuthorize]
        [Route("apply_cancelbooking")]
        [HttpPost]
        public object ApplyCancelTicket(
            long userId,
            string verityCode,
            string account,
            int type)
        {
            using (var db = new MeiHiEntities())
            {
                var user = db.User.FirstOrDefault(a => a.UserId == userId);

                if (user == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        result = "获取用户信息失败"
                    };
                }

                var booking = user.Booking.FirstOrDefault(
                        a => a.VerifyCode == verityCode
                        && a.UserId == userId
                        && a.IsBilling == true
                        && a.IsUsed == false
                        && a.Cancel == false);

                if (booking == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        result = "获取用户订单信息失败"
                    };
                }

                booking.Cancel = true;

                if (type == 0)
                {
                    booking.AlipayAccount = account;
                }
                else if (type == 1)
                {
                    booking.WeiXinAccount = account;
                }

                db.SaveChanges();

                return new
                {
                    jsonStatus = 1,
                    result = "申请退款成功,退款将在次日到账"
                };
            }
        }

        [ApiAuthorize]
        [Route("generate_booking")]
        [HttpPost]
        public object GenerateBooking(
            long userId,
            int count,
            long serviceId,
            decimal cost,
            string designer = null)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var user = db.User.FirstOrDefault(a => a.UserId == userId);

                    if (user == null)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            result = "获取用户信息失败"
                        };
                    }

                    var service = db.Service.FirstOrDefault(a => a.ServiceId == serviceId);

                    if (service == null)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            result = "获取服务失败"
                        };
                    }

                    var booking = new Booking()
                    {
                        ServiceName = service.Title,
                        ShopName = service.Shop.Title,
                        ServiceId = service.ServiceId,
                        ShopId = service.ShopId,
                        Mobile = user.Mobile,
                        UserId = userId,
                        DateModified = DateTime.Now,
                        DateCreated = DateTime.Now,
                        Count = count,
                        Cost = cost,
                        Designer = designer,
                        Status = false,
                        IsUsed = false,
                        IsBilling = false,
                        CancelSuccess = false,
                        Cancel = false,
                        VerifyCode = "",
                        AlipayAccount = "",
                        WeiXinAccount = "",
                    };

                    db.Booking.Add(booking);
                    db.SaveChanges();

                    return new
                    {
                        jsonStatus = 1,
                        result = "订单生产成功 订单号"
                    };
                }
            }
            catch (DbEntityValidationException ex)
            {

                throw;
            }
        }

        [ApiAuthorize]
        [Route("Call_AliPayApiToPayServices")]
        [HttpPost]
        public object CallAliPayApiToPayServices(
            decimal cost,
            long companyId,
            long bookingId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var booking = db.Booking.FirstOrDefault(a => a.BookingId == bookingId);

                    if (booking == null)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            result = "订单不存在"
                        };
                    }

                    //call alipay success 

                    var temp = Guid.NewGuid().ToString().Replace("-", string.Empty);
                    var verifyCode = Regex.Replace(temp, "[a-zA-Z]", string.Empty).Substring(0, 12);
                    booking.VerifyCode = verifyCode;
                    booking.IsBilling = true;
                    db.SaveChanges();
                    LuoSiMaoTextMessage.SendBookingVerifyCode(
                        booking.Mobile,
                        "订购[" + booking.ShopName + "][" + booking.ServiceName + "]服务成功, 美嗨券:" + verifyCode + "【美嗨科技】");

                    return new
                    {
                        jsonStatus = 1,
                        result = "订单支付成功"
                    };
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [ApiAuthorize]
        [Route("Call_WeiXinPayApiToPayServices")]
        [HttpPost]
        public object CallWeiXinPayApiToPayServices(
            decimal cost,
            long companyId,
            long bookingId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var booking = db.Booking.FirstOrDefault(a => a.BookingId == bookingId);

                    if (booking == null)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            result = "订单不存在"
                        };
                    }

                    //call weixin success 

                    var temp = Guid.NewGuid().ToString().Replace("-", string.Empty);
                    var verifyCode = Regex.Replace(temp, "[a-zA-Z]", string.Empty).Substring(0, 12);
                    booking.VerifyCode = verifyCode;
                    booking.IsBilling = true;
                    db.SaveChanges();
                    LuoSiMaoTextMessage.SendBookingVerifyCode(
                        booking.Mobile,
                        "订购[" + booking.ShopName + "][" + booking.ServiceName + "]服务成功, 美嗨券:" + verifyCode + "【美嗨科技】");

                    return new
                    {
                        jsonStatus = 1,
                        result = "订单支付成功"
                    };
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <param name="serviceId"></param>
        /// <param name="cost"></param>
        /// <param name="designer"></param>
        /// <returns></returns>
        [ApiAuthorize]
        [Route("generate_bookings")]
        [HttpPost]
        public object GenerateBookings(GenerateBookingsModel model)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var user = db.User.FirstOrDefault(a => a.UserId == model.UserId);

                    if (user == null)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            result = "获取用户信息失败"
                        };
                    }

                    var tickets = DateTime.Now.Ticks;

                    foreach (var item in model.Details)
                    {
                        var service = db.Service.FirstOrDefault(a => a.ServiceId == item.ServiceId);

                        if (service == null)
                        {
                            return new
                            {
                                jsonStatus = 0,
                                result = "获取服务失败 服务ID:" + item.ServiceId
                            };
                        }

                        var booking = new Booking()
                        {
                            ServiceName = service.Title,
                            ShopName = service.Shop.Title,
                            ServiceId = service.ServiceId,
                            ShopId = service.ShopId,
                            Mobile = user.Mobile,
                            UserId = user.UserId,
                            DateModified = DateTime.Now,
                            DateCreated = DateTime.Now,
                            Count = item.Count,
                            Cost = item.Cost,
                            Designer = string.IsNullOrEmpty(item.Designer) ? null : item.Designer,
                            Status = false,
                            IsUsed = false,
                            IsBilling = false,
                            CancelSuccess = false,
                            Cancel = false,
                            VerifyCode = "",
                            AlipayAccount = "",
                            WeiXinAccount = "",
                            Tickets = tickets
                        };

                        db.Booking.Add(booking);
                    }

                    db.SaveChanges();

                    return new
                    {
                        jsonStatus = 1,
                        result = "批量生产订单成功 批量订单号:" + tickets
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    jsonStatus = 0,
                    result = "生产订单失败:" + ex
                };
            }
        }

        [ApiAuthorize]
        [Route("Call_AliPayApiToPayBookings")]
        [HttpPost]
        public object CallAliPayApiToPayBookings(
            decimal cost,
            long companyId,
            long bookingsId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var bookings = db.Booking.Where(a => a.Tickets == bookingsId);

                    if (bookings == null || bookings.Count() == 0)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            result = "批量订单不存在 BookingsId:" + bookingsId
                        };
                    }

                    if (cost != bookings.Sum(a => a.Cost))
                    {
                        return new
                       {
                           jsonStatus = 0,
                           result = "批量订单号:" + bookingsId + " 支付不成功 请核对价格 订单实际总额为:" + bookings.Sum(a => a.Cost) + " 请求总额为:" + cost
                       };
                    }

                    //call alipay success 
                    var sb = new StringBuilder();

                    foreach (var item in bookings)
                    {
                        var temp = Guid.NewGuid().ToString().Replace("-", string.Empty);
                        var verifyCode = Regex.Replace(temp, "[a-zA-Z]", string.Empty).Substring(0, 12);
                        item.VerifyCode = verifyCode;
                        item.IsBilling = true;
                        sb.Append("订购[" + item.ShopName + "][" + item.ServiceName + "]服务成功, 美嗨券:" + verifyCode);
                    }

                    db.SaveChanges();
                    LuoSiMaoTextMessage.SendBookingVerifyCode(
                        bookings.First().Mobile,
                        sb.ToString() + "【美嗨科技】");

                    return new
                    {
                        jsonStatus = 1,
                        result = "批量支付订单成功：" + sb.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    jsonStatus = 0,
                    result = "批量支付订单失败：" + ex
                };
            }
        }

        [ApiAuthorize]
        [Route("Call_WeiXinPayApiToPayBookings")]
        [HttpPost]
        public object CallWeiXinPayApiToPayBookings(
            decimal cost,
            long companyId,
            long bookingsId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var bookings = db.Booking.Where(a => a.Tickets == bookingsId);

                    if (bookings == null || bookings.Count() == 0)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            result = "批量订单不存在"
                        };
                    }

                    //call weixinpay success 
                    var sb = new StringBuilder();

                    foreach (var item in bookings)
                    {
                        var temp = Guid.NewGuid().ToString().Replace("-", string.Empty);
                        var verifyCode = Regex.Replace(temp, "[a-zA-Z]", string.Empty).Substring(0, 12);
                        item.VerifyCode = verifyCode;
                        item.IsBilling = true;
                        sb.Append("订购[" + item.ShopName + "][" + item.ServiceName + "]服务成功, 美嗨券:" + verifyCode);
                    }

                    db.SaveChanges();
                    LuoSiMaoTextMessage.SendBookingVerifyCode(
                        bookings.First().Mobile,
                        sb.ToString() + "【美嗨科技】");

                    return new
                    {
                        jsonStatus = 1,
                        result = "批量支付订单成功：" + sb.ToString()
                    };
                }
            }
            catch (Exception ex)
            {

                return new
                {
                    jsonStatus = 0,
                    result = "批量支付订单失败：" + ex
                };
            }
        }

        public void CallBackPage()
        {

        }
    }
}
