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
                        resut = "获取用户信息失败"
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
                        resut = "获取用户订单信息失败"
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
                    resut = "申请退款成功,退款将在次日到账"
                };
            }
        }

        [ApiAuthorize]
        [Route("generate_booking")]
        [HttpPost]
        public object GenerateBooking(long userId, int count, long serviceId, decimal cost, string designer = null)
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
                            resut = "获取用户信息失败"
                        };
                    }

                    var service = db.Service.FirstOrDefault(a => a.ServiceId == serviceId);

                    if (service == null)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            resut = "获取服务失败"
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
                        resut = "订单生产成功 订单号"
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
        public object CallAliPayApiToPayServices(decimal cost, long companyId, long bookingId)
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
                            resut = "订单不存在"
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
                        resut = "订单支付成功"
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
        public object CallWeiXinPayApiToPayServices(decimal cost, long companyId, long bookingId)
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
                            resut = "订单不存在"
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
                        resut = "订单支付成功"
                    };
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void CallBackPage()
        {

        }
    }
}
