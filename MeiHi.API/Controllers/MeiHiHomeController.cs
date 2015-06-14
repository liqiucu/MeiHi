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
using System.Web;

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
                    result = temp
                };
            }

            return new
            {
                jsonStatus = 1,
                result = "没有店铺评论"
            };
        }

        [HttpGet]
        [Route("get_allregions")]
        [AllowAnonymous]
        public object GetAllRegionsByCityId(int cityId = 20)
        {
            using (var db = new MeiHiEntities())
            {
                var regions = db.Region.Where(a => a.ParentRegionId == cityId);

                List<RegionModel> models = new List<RegionModel>();

                foreach (var item in regions)
                {
                    models.Add(new RegionModel()
                    {
                        RegionId = item.RegionId,
                        RegionName = item.Name
                    });
                }

                return new
                {
                    jsonStatus = 1,
                    result = models
                };
            }
        }

        /// <summary>
        /// 搜索店铺
        /// </summary>
        /// <param name="shopName"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_searchshop")]
        [AllowAnonymous]
        public object SearchShop(string shopName, string start)
        {
            using (var db = new MeiHiEntities())
            {
                if (HttpRuntime.Cache.Get(start) as List<ShopDistanceModel> == null)
                {
                    new ShopController().CalDistance(start);
                }

                var shopDistances = HttpRuntime.Cache.Get(start) as List<ShopDistanceModel>;

                if (shopDistances == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        result = "内存爆了 请联系 13167226393"
                    };
                }

                var shops = ShopLogic.GetAllShopsFromCache();

                var temp = shops.Where(a => a.Title.Contains(shopName));

                if (temp == null || temp.Count() == 0)
                {
                    return new
                    {
                        jsonStatus = 1,
                        result = "没有搜索到店铺"
                    };
                }

                foreach (var item in temp)
                {
                    item.Distance = shopDistances.First(a => a.ShopId == item.ShopId).Distance;
                }

                return new
                {
                    jsonStatus = 1,
                    result = temp.OrderBy(a=>a.Distance)
                };
            }
        }

        /// <summary>
        /// 搜索店铺
        /// </summary>
        /// <param name="shopName"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_searchshopOrderByDiscountRate")]
        [AllowAnonymous]
        public object SearchShopOrderByDiscountRate(string shopName, string start)
        {
            using (var db = new MeiHiEntities())
            {
                if (HttpRuntime.Cache.Get(start) as List<ShopDistanceModel> == null)
                {
                    new ShopController().CalDistance(start);
                }

                var shopDistances = HttpRuntime.Cache.Get(start) as List<ShopDistanceModel>;

                if (shopDistances == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        result = "内存爆了 请联系 13167226393"
                    };
                }

                var shops = ShopLogic.GetAllShopsFromCache();

                var temp = shops.Where(a => a.Title.Contains(shopName));

                if (temp == null || temp.Count() == 0)
                {
                    return new
                    {
                        jsonStatus = 1,
                        result = "没有搜索到店铺"
                    };
                }

                foreach (var item in temp)
                {
                    item.Distance = shopDistances.First(a => a.ShopId == item.ShopId).Distance;
                }

                return new
                {
                    jsonStatus = 1,
                    result = temp.OrderBy(a=>a.DiscountRate)
                };
            }
        }

        /// <summary>
        /// 搜索店铺
        /// </summary>
        /// <param name="shopName"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("search_shops_byregion")]
        [AllowAnonymous]
        public object SearchShopsByRegion(int regionId, string start)
        {
            using (var db = new MeiHiEntities())
            {
                var temp = db.Shop.Where(a => a.RegionID == regionId);

                if (temp != null)
                {
                    List<ShopModel> results = new List<ShopModel>();

                    foreach (var item in temp)
                    {
                        results.Add(new ShopModel()
                        {
                            Coordinates = item.Coordinates,
                            ShopId = item.ShopId,
                            Title = item.Title,
                            DiscountRate = ShopLogic.GetDiscountRate(item.ShopId),
                            RegionName = item.StreetId != null ? item.Region1.Name : item.Region.Name,
                            ShopImageUrl = item.ShopBrandImages.FirstOrDefault().url,
                            Rate = ShopLogic.GetShopRate(item.ShopId),
                            ParentShopId = item.ParentShopId,
                            Distance = HttpUtils.CalOneShop(start, item.Coordinates)
                        });
                    }

                    return new
                    {
                        jsonStatus = 1,
                        result = results
                    };
                }

                return new
                {
                    jsonStatus = 1,
                    result = "没有搜索到店铺"
                };
            }
        }

        [HttpGet]
        [Route("Show_homeadd")]
        [AllowAnonymous]
        public object GetHomepageAddImageUrl()
        {
            using (var db = new MeiHiEntities())
            {
                var add = 
                    db.Add.Select(a => a.Url).FirstOrDefault();

                if (!string.IsNullOrEmpty(add))
                {
                    return new
                    {
                        jsonStatus = 1,
                        result = add
                    };
                }
                return new
                {
                    jsonStatus = 1,
                    result = "没有首页广告图片"
                };
            }
        }
    }
}
