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


namespace MeiHi.API.Controllers
{
    [RoutePrefix("meihihome")]
    public class MeiHiHomeController : ApiController
    {
        /// <summary>
        /// 获取首页所有评论,分页
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_homepageComments")]
        [AllowAnonymous]
        public object GetAllServiceComments(int page, int size)
        {
            var temp = ShopLogic.GetAllUserComments(page, size);

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
                resut = "没有店铺评论"
            };
        }

        /// <summary>
        /// 搜索店铺
        /// </summary>
        /// <param name="shopName"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_searchshop")]
        [AllowAnonymous]
        public object SearchShop(string shopName, string region)
        {
            using (var db = new MeiHiEntities())
            {
                var temp = db.Shop.Where(a => a.Title.Contains(shopName)).FirstOrDefault();
                
                if (temp != null)
                {
                    var shopModel = new ShopModel()
                    {
                        Coordinates = temp.Coordinates,
                        ShopId = temp.ShopId,
                        Title = temp.Title,
                        DiscountRate = ShopLogic.GetDiscountRate(temp.ShopId),
                        RegionName = temp.Region.Name,
                        ShopImageUrl = temp.ShopBrandImages.FirstOrDefault().url,
                        Rate = ShopLogic.GetShopRate(temp.ShopId),
                        ParentShopId = temp.ParentShopId,
                        Distance = HttpUtils.CalOneShop(region, temp.Coordinates)
                    };

                    return new
                    {
                        jsonStatus = 1,
                        resut = shopModel
                    };
                }

                return new
                {
                    jsonStatus = 1,
                    resut = "没有搜索到店铺"
                };
            }
        }
    }
}
