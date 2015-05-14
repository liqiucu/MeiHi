using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MeiHi.Model;
using MeiHi.API.Models;
using System.Threading.Tasks;
using MeiHi.API.Helper;
using Newtonsoft.Json;
using System.Web.Caching;
using MeiHi.API.Logic;
using MeiHi.API.Models.Service;

namespace MeiHi.API.Controllers
{
    [RoutePrefix("shervice")]
    public class ServiceController : ApiController
    {
        [HttpGet]
        [Route("show_service")]
        [AllowAnonymous]
        public object GetService(long serviceId)
        {
            using (var db = new MeiHiEntities())
            {
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

                    return new
                    {
                        jsonStatus = 1,
                        resut = serviceModel
                    };
                }

                return new
                {
                    jsonStatus = 0,
                    resut = "获取服务信息失败"
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
                    resut = temp
                };
            }

            return new
            {
                jsonStatus = 1,
                resut = "没有服务评论"
            };
        }

        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [ApiAuthorize]
        [HttpPost]
        [Route("Add_Favorite")]
        public object AddToFavorite(long serviceId,long userId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    db.UserFavorites.Add(new UserFavorites()
                    {
                        UserId = userId,
                        ServiceId = serviceId
                    });

                    db.SaveChanges();

                    return new
                    {
                        jsonStatus = 1,
                        resut = "添加成功"
                    };
                }
            }
            catch (Exception)
            {
                 return new
                {
                    jsonStatus = 0,
                    resut = "添加失败"
                };
                throw;
            }
        }
    }
}
