using MeiHi.API.Helper;
using MeiHi.API.Logic;
using MeiHi.API.ViewModels;
using MeiHi.CommonDll.Helper;
using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MeiHi.API.Controllers
{
    [RoutePrefix("user")]
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

                    var userProfilePhotoImage = ImageHelper.SaveImage(
                            Request.RequestUri.Authority,
                            "/upload/user/" + userId + "/",
                            bodyparts);

                    if (!string.IsNullOrEmpty(user.ProfilePhoto))
                    {

                        ImageHelper.DeleteImageFromDataBaseAndPhyclePath(user.ProfilePhoto, "/upload/user/" + userId + "/");
                    }

                    user.ProfilePhoto = userProfilePhotoImage.FirstOrDefault();
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
                var tickets = db.Booking.Where(a => a.UserId == userId && a.IsBilling);

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
                model.UsedConsumeMeiHiTickets = new List<MeiHiTicketModel>();
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

                    if (item.IsUsed)
                    {
                        temp.IsUsed = true;
                        model.UsedConsumeMeiHiTickets.Add(temp);
                    }
                    else
                    {
                        if (!item.Cancel)
                        {
                            temp.Comment = "已付款但未使用未退款";
                            temp.Cancel = false;
                            temp.IsUsed = false;
                            temp.CancelSuccess = false;
                            model.NotUsedConsumeMeiHiTickets.Add(temp);
                        }

                        if (item.Cancel && !item.CancelSuccess)
                        {
                            temp.Comment = "已付款并已经申请退款";
                            temp.IsUsed = false;
                            temp.Cancel = true;
                            temp.CancelSuccess = false;
                            model.CalceledConsumeMeiHiTickets.Add(temp);
                        }

                        if (item.Cancel && item.CancelSuccess)
                        {
                            temp.Comment = "申请退款成功";
                            temp.CancelSuccess = true;
                            temp.Cancel = true;
                            temp.IsUsed = false;
                            model.CalceledConsumeMeiHiTickets.Add(temp);
                        }
                    }
                }

                return new
                {
                    jsonStatus = 1,
                    resut = model
                };
            }
        }

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
        [Route("get_NotPayedBookings")]
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

        /// <summary>
        /// 订单号和服务对应唯一的 一个
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        [ApiAuthorize]
        [Route("get_NotPayedBooking")]
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
                        resut = "订单异常了,订单号：" + bookingId + " 服务名：" + serviceName
                    };
                }

                if (booking.IsBilling)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "这单已经支付了"
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

        [ApiAuthorize]
        [Route("get_UserFavorites")]
        [HttpGet]
        public object GetUserFavorites(long userId, string region)
        {
            using (var db = new MeiHiEntities())
            {
                var user = db.User.FirstOrDefault(a => a.UserId == userId);

                if (user == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "没有获取到该用户信息"
                    };
                }

                var shops = new List<ShopModel>();
                var services = new List<ServiceModel>();

                if (user.UserFavorites != null)
                {
                    var shopTemp = user.UserFavorites.Where(a => a.ShopId > 0);

                    foreach (var item in shopTemp)
                    {
                        shops.Add(new ShopModel()
                        {
                            Coordinates = item.Shop.Coordinates,
                            ShopId = item.Shop.ShopId,
                            Title = item.Shop.Title,
                            DiscountRate = ShopLogic.GetDiscountRate(item.Shop.ShopId),
                            RegionName = ShopLogic.GetRegionName(item.Shop.RegionID, item.Shop.ShopId),
                            ShopImageUrl = item.Shop.ShopBrandImages.Count > 0
                                            ? item.Shop.ShopBrandImages.FirstOrDefault().url
                                            : null,
                            Rate = ShopLogic.GetShopRate(item.Shop.ShopId),
                            Distance = HttpUtils.CalOneShop(region, item.Shop.Coordinates)
                        });
                    }

                    var serviceTemp = user.UserFavorites.Where(a => a.ServiceId > 0);

                    foreach (var item in serviceTemp)
                    {
                        services.Add(new ServiceModel()
                        {
                            ServiceId = item.ServiceId.Value,
                            Title = item.Service.Title,
                            TitleUrl = item.Service.TitleUrl,
                            CMUnitCost = item.Service.CMUnitCost,
                            OriginalUnitCost = item.Service.OriginalUnitCost
                        });
                    }
                }

                var model = new UserFavoritesModel()
                {
                    UserId = userId,
                    FavoritieSerivces = services,
                    FavoritieShops = shops
                };

                return new
                {
                    jsonStatus = 1,
                    resut = model
                };
            }
        }

        [ApiAuthorize]
        [Route("get_userAccountInfo")]
        [HttpGet]
        public object GetUserAccountInfo(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var user = db.User.FirstOrDefault(a => a.UserId == userId);

                if (user == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "没有获取到该用户信息"
                    };
                }

                var useraccountinfo = new UserAccountInfoModel()
                {
                    Balance = user.Balance,
                    Mobile = user.Mobile,
                    UserId = user.UserId
                };

                return new
                {
                    jsonStatus = 1,
                    resut = useraccountinfo
                };
            }
        }

        [Route("post_UserSuggest")]
        [HttpPost]
        [AllowAnonymous]
        public object PostUserSuggest(string content, string contract)
        {
            using (var db = new MeiHiEntities())
            {
                if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(contract))
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "不能为空"
                    };
                }

                db.UserSuggest.Add(new UserSuggest()
                {
                    Content = content,
                    Contract = contract,
                    DateCreated = DateTime.Now
                });

                db.SaveChanges();

                return new
                {
                    jsonStatus = 1,
                    resut = "提议成功"
                };
            }
        }

        [ApiAuthorize]
        [Route("post_usercomment")]
        [HttpPost]
        public async Task<object> PostUserComment(long userId, string content, int rate, long serviceId, long shopId)
        {
            try
            {
                // 设置上传目录
                string tempPath = "/upload/temp/";
                var provider = new MultipartFormDataStreamProvider(HttpContext.Current.Server.MapPath(tempPath));

                // 接收数据，并保存文件
                var bodyparts = await Request.Content.ReadAsMultipartAsync(provider);

                if (string.IsNullOrEmpty(content) || serviceId == 0 || shopId == 0 || userId == 0)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "参数异常"
                    };
                }

                using (var db = new MeiHiEntities())
                {
                    var comment = new UserComments()
                    {
                        Comment = content,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        Display = true,
                        Rate = rate,
                        ServiceId = serviceId,
                        ShopId = shopId,
                        ServiceName = db.Service.First(a => a.ServiceId == serviceId).Title,
                        UserId = userId,
                        UserCommentSharedImg = new List<UserCommentSharedImg>()
                    };

                    if (bodyparts != null
                        && bodyparts != null
                        && bodyparts.FileData != null
                        && bodyparts.FileData.Count > 0)
                    {
                        var sharesImageUrls = ImageHelper.SaveImage(
                            Request.RequestUri.Authority,
                            "/upload/user/" + userId + "/comments/",
                            bodyparts);

                        foreach (var url in sharesImageUrls)
                        {
                            comment.UserCommentSharedImg.Add(new UserCommentSharedImg()
                            {
                                DateCreated = DateTime.Now,
                                DateModified = DateTime.Now,
                                ImgUrl = url
                            });
                        }
                    }

                    db.UserComments.Add(comment);
                    db.SaveChanges();

                    return new
                    {
                        jsonStatus = 1,
                        resut = "评论成功"
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    jsonStatus = 0,
                    resut = ex.Message
                };
            }
        }
    }
}