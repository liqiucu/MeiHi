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
    [RoutePrefix("shop")]
    public class ShopController : ApiController
    {
        [HttpGet]
        [Route("recmmondShops")]
        [AllowAnonymous]
        public object GetRecmmondShoppings(string region)
        {
            using (var db = new MeiHiEntities())
            {
                var recmmondShops = db.RecommandShop.Take(10);

                if (recmmondShops == null || recmmondShops.Count() == 0)
                {
                    return new
                    {
                        jsonStatus = 0,
                        result = "没有推荐店铺"
                    };
                }

                var shops = from a in db.Shop
                            where recmmondShops.Select(b => b.RecommandShopId).Contains(a.ShopId)
                            select a;

                var results = new List<ShopModel>();
                
                foreach (var item in shops)
                {
                    var shopModel = new ShopModel()
                    {
                        Coordinates = item.Coordinates,
                        ShopId = item.ShopId,
                        Title = item.Title,
                        DiscountRate = ShopLogic.GetDiscountRate(item.ShopId),
                        RegionName = ShopLogic.GetRegionName(item.RegionID, item.ShopId),
                        ShopImageUrl = ShopLogic.GetShopImageUrl(item.ProductBrandId),
                        Rate = ShopLogic.GetShopRate(item.ShopId),
                        Distance = HttpUtils.CalOneShop(region, item.Coordinates)
                    };

                    results.Add(shopModel);
                }

                return new
                {
                    jsonStatus = 1,
                    resut = results
                };
            }
        }

        /// <summary>
        /// 距离最近
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_NearestDistanceShop")]
        [AllowAnonymous]
        public object GetNearestDistanceShops(int page = 1, int size = 20)
        {

            if (System.Web.HttpContext.Current.Session["Shops"] == null)
            {
                return new
                  {
                      jsonStatus = 0,
                      resut = "网络较慢，还没刷出店铺信息"
                  };
            }
            var Shops = System.Web.HttpContext.Current.Session["Shops"] as List<ShopModel>;

            var shops = Shops.OrderBy(a => a.Distance).Skip((page - 1) * size).Take(size);

            return new
            {
                jsonStatus = 1,
                resut = shops
            };
        }

        /// <summary>
        /// 折扣最低
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_LowestDiscountRateShops")]
        [AllowAnonymous]
        public object GetLowestDiscountRateShops(int page = 1, int size = 20)
        {
            if (System.Web.HttpContext.Current.Session["Shops"] == null)
            {
                return new
                {
                    jsonStatus = 0,
                    resut = "网络较慢，还没刷出店铺信息"
                };
            }

            var Shops = System.Web.HttpContext.Current.Session["Shops"] as List<ShopModel>;
            var shops = Shops.OrderBy(a => a.DiscountRate).Skip((page - 1) * size).Take(size);

            return new
            {
                jsonStatus = 1,
                resut = shops
            };
        }

        /// <summary>
        /// 店铺的服务详情页
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_ShopServices")]
        [AllowAnonymous]
        public object GetShopServices(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();
                if (shop != null)
                {
                    var shopModel = new ShopModel()
                    {
                        Coordinates = shop.Coordinates,
                        DetailAddress = shop.DetailAddress,
                        Phone = shop.Phone,
                        ShopTag = shop.ShopTag,
                        ShopId = shop.ShopId,
                        Title = shop.Title,
                        ShopImageUrl = ShopLogic.GetShopImageUrl(shop.ProductBrandId),
                        ProductBrandCount = ShopLogic.GetProductBrandCount(shop.ProductBrandId),
                        Services = ShopLogic.GetServices(shop.ShopId)
                    };
                    return new
                    {
                        jsonStatus = 1,
                        resut = shopModel
                    };
                }

                return new
                {
                    jsonStatus = 0,
                    resut = "获取店铺服务列表失败"
                };
            }
        }

        /// <summary>
        /// 店铺详情页
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_ShopDetails")]
        [AllowAnonymous]
        public object GetShopDetails(long shopId)
        {
            using (var db = new MeiHiEntities())
            {

                var shop = db.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();
                if (shop != null)
                {
                    var shopModel = new ShopModel()
                    {
                        Coordinates = shop.Coordinates,
                        DetailAddress = shop.DetailAddress,
                        Phone = shop.Phone,
                        ShopId = shop.ShopId,
                        ProductBrandImages = ShopLogic.GetProductBrandImages(shopId),
                        PurchaseNotes = shop.PurchaseNotes,
                        ParentShopId = shop.ParentShopId,//当要调用Show_BranchShops时候传递ParentShopId
                        BranchStoreCount = ShopLogic.GetBranchStoreCount(shop.ParentShopId),//如果查询出来有分店 那么就需要调用 接口Show_BranchShops
                        UserComments = ShopLogic.GetUserCommentsTopFiveByShopId(shop.ShopId)
                    };
                    return new
                    {
                        jsonStatus = 1,
                        resut = shopModel
                    };
                }

                return new
                {
                    jsonStatus = 0,
                    resut = "获取店铺服务列表失败"
                };
            }
        }

        /// <summary>
        /// 获取店铺所有的评论
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_ShopAllComments")]
        [AllowAnonymous]
        public object GetAllShopComments(long shopId, int page, int size)
        {
                var temp = ShopLogic.GetAllUserCommentsByShopId(shopId, page,size);

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
        /// 分店列表
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_BranchShops")]
        [AllowAnonymous]
        public object GetBranchShops(long shopId)
        {
            if (System.Web.HttpContext.Current.Session["Shops"] == null)
            {
                return new
                {
                    jsonStatus = 0,
                    resut = "网络较慢，还没刷出店铺信息"
                };
            }

            var Shops = System.Web.HttpContext.Current.Session["Shops"] as List<ShopModel>;
            var shops = Shops.Where(a => a.ParentShopId == shopId);

            return new
            {
                jsonStatus = 1,
                resut = shops
            };
        }

        /// <summary>
        /// 店铺的图片列表
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_ShopProductImages")]
        [AllowAnonymous]
        public object GetShopProductImages(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();//ShopLogic.GetShopProductBrandImages(shopId);

                if (shop != null && shop.ShopBrandImages != null && shop.ShopBrandImages.Count>0)
                {
                    return new
                    {
                        jsonStatus = 1,
                        resut = shop.ShopBrandImages.Select(a=>a.url)
                    };
                }

                return new
                {
                    jsonStatus = 0,
                    resut = "店铺没有品牌图片"
                };
            }
        }


        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="userId"></param>
        [ApiAuthorize]
        [HttpPost]
        [Route("Add_Favorite")]
        public object AddToFavorite(long shopId, long userId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    db.UserFavorites.Add(new UserFavorites()
                    {
                        UserId = userId,
                        ShopId = shopId
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

        /// <summary>
        /// 计算所有店铺距离和折扣并且排序
        /// </summary>
        /// <param name="region">用户所在地的经纬度</param>
        [AllowAnonymous]
        [Route("shop_caldistance")]
        [HttpGet]
        public object CalDistance(string region)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var shops = db.Shop.Where(a => a.IsOnline == true).ToList();
                    var shopResults = new List<ShopModel>();

                    for (int j = 0; j < shops.Count(); j++)
                    {
                        var shopModel = new ShopModel()
                        {
                            Coordinates = shops[j].Coordinates,
                            ShopId = shops[j].ShopId,
                            Title = shops[j].Title,
                            DiscountRate = ShopLogic.GetDiscountRate(shops[j].ShopId),
                            RegionName = shops[j].Region.Name,// ShopLogic.GetRegionName(shops[j].RegionID, shops[j].ShopId),
                            ShopImageUrl = shops[j].ShopBrandImages.FirstOrDefault().url,
                            Rate = ShopLogic.GetShopRate(shops[j].ShopId),
                            ParentShopId = shops[j].ParentShopId,
                            Distance = HttpUtils.CalOneShop(region, shops[j].Coordinates)
                        };

                        shopResults.Add(shopModel);
                    }

                    System.Web.HttpContext.Current.Session["Shops"] = shopResults;

                    return new
                    {
                        jsonStatus = 1,
                        resut = shopResults
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    jsonStatus = 0,
                    resut = ex
                };
                throw;
            }
        }
    }
}
