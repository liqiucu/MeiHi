using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MeiHi.Model;
using MeiHi.API.ViewModels;
using MeiHi.API.Logic;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Drawing;
using MeiHi.API.Helper;
using System.Text;
using MeiHi.API.Models.UserComments;
using MeiHi.API.Models.Booking;

namespace MeiHi.API.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        [Route("Show_UserHome")]
        public object UserHome(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var user = UserLogic.GetUserByUserId(userId);

                if (user == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "用户不存在"
                    };
                }

                var userHomeModel = new UserHomeModel()
                {
                    Balance = user.Balance,
                    MeiHiBookingVerifyCodeCount = db.Booking.Count(a => a.UserId == userId),
                    MeiHiUserName = user.FullName,
                    UserId = userId,
                    UserCommentCount = UserLogic.GetUserCommentsByUserId(userId).Count,
                    UserImageUrl = user.ProfilePhoto
                };

                return new
                {
                    jsonStatus = 1,
                    resut = userHomeModel
                };
            }
        }

        [HttpGet]
        [Route("Show_UserDetail")]
        public object UserDetail(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var user = UserLogic.GetUserByUserId(userId);

                if (user == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "用户不存在"
                    };
                }

                var userDetailModel = new UserDetailModel()
                {
                    Address = user.Address,
                    BirthDay = user.BirthDay,
                    Gender = user.Gender,
                    HairLenth = user.HairOfLegth,
                    MeiHiUserName = user.FullName,
                    UserId = user.UserId,
                    UserImageUrl = user.ProfilePhoto
                };

                return new
                {
                    jsonStatus = 1,
                    resut = userDetailModel
                };
            }
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("Update_UserDetail")]
        public object UpdateUserDetail(
            long userId,
            string userFullName,
            int hairLenth,
            DateTime birthday,
            int gender,
            string address)
        {
            using (var db = new MeiHiEntities())
            {
                var user = db.User.FirstOrDefault(a => a.UserId == userId);

                if (user == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "用户不存在"
                    };
                }

                string token = Request.Headers.GetValues("Authorization").First();

                if (user.Token != token)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "用户Token和用户ID不一致 USERID：" + userId
                    };
                }

                user.FullName = userFullName;
                user.HairOfLegth = hairLenth;
                user.BirthDay = birthday;
                user.Gender = gender;
                user.Address = address;

                db.SaveChanges();

                return new
                {
                    jsonStatus = 1,
                    resut = "用户信息更新成功"
                };
            }
        }

        [ApiAuthorize]
        [Route("upload_userprofile")]
        [HttpPost]
        public async Task<object> UploadProfile(long userId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {

                    // 设置上传目录
                    string tempPath = "/upload/temp/";
                    var provider = new MultipartFormDataStreamProvider(HttpContext.Current.Server.MapPath(tempPath));

                    // 接收数据，并保存文件
                    var bodyparts = await Request.Content.ReadAsMultipartAsync(provider);

                    if (bodyparts.FileData.Count == 0)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            resut = "图片上传数量为空"
                        };
                    }

                    if (bodyparts.FileData.Count > 1)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            resut = "图片上传数量不该大于1"
                        };
                    }

                    var user = db.User.FirstOrDefault(r => r.UserId == userId);

                    if (user == null)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            resut = "用户不存在 UserId:" + userId
                        };
                    }

                    string token = Request.Headers.GetValues("Authorization").First();

                    if (user.Token != token)
                    {
                        return new
                        {
                            jsonStatus = 0,
                            resut = "用户Token和用户ID不一致 USERID：" + userId
                        };
                    }

                    string extension = ".jpg";
                    string newPath = "/upload/user/" + userId + "/";
                    string fileName = Guid.NewGuid().ToString();

                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(newPath)))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(newPath));
                    }

                    var filePhysicalPath = HttpContext.Current.Server.MapPath(newPath + fileName + extension);
                    File.Move(bodyparts.FileData[0].LocalFileName, filePhysicalPath);
                    Image image = Image.FromFile(filePhysicalPath);

                    using (var resized = ImageHelper.ResizeImage(image, 100, 100))
                    {
                        //save the resized image as a jpeg with a quality of 90
                        ImageHelper.SaveJpeg(HttpContext.Current.Server.MapPath(newPath + fileName + "_100_100" + extension), resized, 100);
                    }

                    image.Dispose();
                    user.ProfilePhoto = newPath + fileName + extension;
                    user.DateModified = DateTime.Now;
                    db.SaveChanges();

                    return new
                    {
                        jsonStatus = 1,
                        resut = "用户上传图片成功 UserId:" + userId
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    jsonStatus = 0,
                    resut = "用户上传图片失败 UserId:" + userId + " ex:" + ex
                };
            }
        }

        [ApiAuthorize]
        [Route("get_usercomments")]
        [HttpPost]
        public object GetUserComments(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var comments = UserLogic.GetUserCommentsByUserId(userId);

                List<UserCommentsModel> result = new List<UserCommentsModel>();

                foreach (var item in comments)
                {
                    var temp = item.UserCommentSharedImg.Where(a => a.UserCommentId == item.UserCommentId);
                    var temp1 = item.UserCommentsReply.Where(a => a.UserCommentId == item.UserCommentId);

                    result.Add(new UserCommentsModel()
                    {
                        Comment = item.Comment,
                        DateCreated = item.DateCreated,
                        Rate = item.Rate,
                        ServiceName = item.ServiceName,
                        ShopId = item.ShopId,
                        UserFullName = item.User.FullName,
                        UserSharedImgaeList = temp != null && temp.Count() > 0 ? temp.Select(a => a.ImgUrl).ToList() : null,
                        MeiHiReply = temp1 != null && temp1.Count() > 0 ? temp1.Select(a => a.Comment).ToList() : null,
                    });
                }

                return new
                {
                    jsonStatus = 1,
                    resut = result
                };
            }
        }

        [ApiAuthorize]
        [Route("get_UserMeiHiTicket")]
        [HttpGet]
        public object GetUserMeiHiTicket(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var tickets = db.Booking.Where(a => a.UserId == userId && a.IsBilling && !a.IsUsed);

                if (tickets == null || tickets.Count() == 0)
                {
                    return new
                    {
                        jsonStatus = 1,
                        resut = "该用户没有订单:" + userId
                    };
                }

                UserMeiHiTicketsModel model = new UserMeiHiTicketsModel();
                model.NotUsedConsumeMeiHiTickets = new List<MeiHiTicketModel>();
                model.CalceledConsumeMeiHiTickets = new List<MeiHiTicketModel>();
                model.UserId = userId;

                foreach (var item in tickets)
                {
                    MeiHiTicketModel temp = new MeiHiTicketModel()
                    {
                        BookingId = item.BookingId,
                        Cost = item.Cost,
                        DateModified = item.DateModified,
                        Count = item.Count,
                        Mobile = item.Mobile,
                        VerifyCode = item.VerifyCode,
                        ServiceName = item.ServiceName,
                        ShopName = item.ShopName
                    };

                    if (!item.Cancel)
                    {
                        temp.Comment = "已付款但未使用";
                        model.NotUsedConsumeMeiHiTickets.Add(temp);
                    }

                    if (item.Cancel && !item.CancelSuccess)
                    {
                        temp.Comment = "已付款并已经申请退款";
                        model.CalceledConsumeMeiHiTickets.Add(temp);
                    }

                    if (item.Cancel && item.CancelSuccess)
                    {
                        temp.Comment = "已付款并退款成功";
                        model.CalceledConsumeMeiHiTickets.Add(temp);
                    }
                }

                return new
                {
                    jsonStatus = 1,
                    resut = model
                };
            }
        }

        [ApiAuthorize]
        [Route("get_getNotPayedBookings")]
        [HttpGet]
        public object GetNotPayedBookings(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var notPayedBookings = db.Booking.Where(a => a.UserId == userId && !a.IsBilling);

                if (notPayedBookings == null && notPayedBookings.Count() == 0)
                {
                    return new
                    {
                        jsonStatus = 1,
                        resut = "该用户没有未支付的订单 userid:" + userId
                    };
                }

                NotPayedBookingsModel result = new NotPayedBookingsModel();
                result.UserId = userId;
                result.NotPayedBookings = new List<NotPayedBookingModel>();

                foreach (var item in notPayedBookings)
                {
                    var temp = new NotPayedBookingModel()
                    {
                        BookingId = item.BookingId,
                        Count = item.Count,
                        ServiceName = item.ServiceName,
                        ShopName = item.ShopName,
                        TotalCost = item.Cost
                    };

                    result.NotPayedBookings.Add(temp);
                }

                return new
                {
                    jsonStatus = 1,
                    resut = result
                };
            }
        }

        [ApiAuthorize]
        [Route("get_getNotPayedBooking")]
        [HttpGet]
        public object GetNotPayedBooking(long bookingId, string serviceName)
        {
            using (var db = new MeiHiEntities())
            {
                var booking = db.Booking.FirstOrDefault(a => a.BookingId == bookingId && a.ServiceName == serviceName);

                if (booking == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "订单异常了 订单号：" + bookingId + " 服务名：" + serviceName + " 支付状态：" + booking.IsBilling
                    };
                }

                if (booking.IsBilling)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "表坑我, 这单已经支付了"
                    };
                }

                var model = new MeiHiTicketModel()
                {
                    BookingId = booking.BookingId,
                    ServiceName = booking.ServiceName,
                    ShopName = booking.ShopName,
                    BuyCounts = booking.Count,
                    Cost = booking.Cost,
                    DataCreate = booking.DateCreated,
                    Desinger = booking.Designer,
                    Mobile = booking.Mobile,
                    MeiHiUnitCost = booking.Service.CMUnitCost,
                    OriginalUnitCost = booking.Service.OriginalUnitCost,
                    Count = booking.Count
                };

                return new
                {
                    jsonStatus = 1,
                    resut = model
                };
            }
        }
    }
}