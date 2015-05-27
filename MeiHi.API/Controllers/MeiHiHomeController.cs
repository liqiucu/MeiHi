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
                    resut = models
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
                        Distance = HttpUtils.CalOneShop(start, temp.Coordinates)
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
                            RegionName = item.Region.Name,
                            ShopImageUrl = item.ShopBrandImages.FirstOrDefault().url,
                            Rate = ShopLogic.GetShopRate(item.ShopId),
                            ParentShopId = item.ParentShopId,
                            Distance = HttpUtils.CalOneShop(start, item.Coordinates)
                        });
                    }

                    return new
                    {
                        jsonStatus = 1,
                        resut = results
                    };
                }

                return new
                {
                    jsonStatus = 1,
                    resut = "没有搜索到店铺"
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
                var add = db.Add.Select(a => a.Url).FirstOrDefault();

                if (!string.IsNullOrEmpty(add))
                {
                    return new
                    {
                        jsonStatus = 1,
                        resut = add
                    };
                }
                return new
                {
                    jsonStatus = 1,
                    resut = "没有首页广告图片"
                };
            }
        }
    }
}
