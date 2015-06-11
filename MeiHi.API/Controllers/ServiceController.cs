using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MeiHi.Model;
using MeiHi.API.ViewModels;
using System.Threading.Tasks;
using MeiHi.API.Helper;
using Newtonsoft.Json;
using System.Web.Caching;
using MeiHi.API.Logic;

namespace MeiHi.API.Controllers
{
    [RoutePrefix("shervice")]
    public class ServiceController : ApiController
    {
        [HttpGet]
        [Route("show_service")]
        [AllowAnonymous]
        public object GetService(long serviceId, long userId = 0)
        {
            using (var db = new MeiHiEntities())
            {
                bool haveAddedToFavorite = false;

                if (userId != 0)
                {
                    haveAddedToFavorite = db.UserFavorites.FirstOrDefault(a => a.ServiceId == serviceId && a.UserId == userId) != null;
                }

                var service = db.Service.Where(a => a.ServiceId == serviceId).FirstOrDefault();

                if (service != null)
                {
                    ServiceModel serviceModel = new ServiceModel();
                    serviceModel.CMUnitCost = service.CMUnitCost;
                    serviceModel.Designer = service.Designer;
                    serviceModel.Detail = service.Detail;
                    serviceModel.IfSupportRealTimeRefund = service.IfSupportRealTimeRefund;
                    serviceModel.OriginalUnitCost = service.OriginalUnitCost;
                    serviceModel.PurchaseNotes = service.Shop.PurchaseNotes;
                    serviceModel.ServiceId = service.ServiceId;
                    serviceModel.ShopId = service.ShopId;
                    serviceModel.Title = service.Title;
                    serviceModel.TitleUrl = service.TitleUrl;
                    serviceModel.UserComments = ShopLogic.GetUserCommentsTopFiveByServiceId(service.ServiceId);
                    serviceModel.HaveAddedToFavorite = haveAddedToFavorite;

                    return new
                    {
                        jsonStatus = 1,
                        result = serviceModel
                    };
                }

                return new
                {
                    jsonStatus = 0,
                    result = "获取服务信息失败"
                };
            }
        }

        /// <summary>
        /// 获取服务的所有评论
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_serviceAllComments")]
        [AllowAnonymous]
        public object GetAllServiceComments(long serviceId, int page, int size)
        {
            var temp = ShopLogic.GetAllUserCommentsByServiceId(serviceId, page, size);

            if (temp != null && temp.Count > 0)
            {
                return new
                {
                    jsonStatus = 1,
                    result = temp
                };
            }

            return new
            {
                jsonStatus = 1,
                result = "没有服务评论"
            };
        }

        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="userId"></param>
        [ApiAuthorize]
        [HttpPost]
        [Route("Add_Favorite")]
        public object AddToFavorite(long serviceId, long userId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var favorite = db.UserFavorites.FirstOrDefault(a => a.ServiceId == serviceId && a.UserId == userId);

                    if (favorite != null)
                    {
                        return new
                        {
                            jsonStatus = 1,
                            result = "添加失败: 已经添加过了"
                        };
                    }

                    db.UserFavorites.Add(new UserFavorites()
                    {
                        UserId = userId,
                        ServiceId = serviceId,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now
                    });

                    db.SaveChanges();

                    return new
                    {
                        jsonStatus = 1,
                        result = "添加成功"
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    jsonStatus = 0,
                    result = "添加失败" + ex
                };
            }
        }

        /// <summary>
        ///取消收藏
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="userId"></param>
        [ApiAuthorize]
        [HttpPost]
        [Route("Cancel_Favorite")]
        public object CancelFavorite(long serviceId, long userId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var favorite = db.UserFavorites.FirstOrDefault(a => a.ServiceId == serviceId && a.UserId == userId);

                    if (favorite == null)
                    {
                        return new
                        {
                            jsonStatus = 1,
                            result = "取消失败：因为没收藏过"
                        };
                    }

                    db.UserFavorites.Remove(favorite);

                    db.SaveChanges();

                    return new
                    {
                        jsonStatus = 1,
                        result = "取消成功"
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    jsonStatus = 0,
                    result = "取消失败" + ex
                };
            }
        }
    }
}
