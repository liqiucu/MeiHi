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
    [RoutePrefix("shop")]
    public class ShopController : ApiController
    {
        [HttpGet]
        [Route("recmmondShops")]
        [AllowAnonymous]
        public object GetRecmmondShoppings(string region)
        {
            try
            {
                if (HttpRuntime.Cache.Get(region) as List<ShopDistanceModel> == null)
                {
                    CalDistance(region);
                }

                var shopDistances = HttpRuntime.Cache.Get(region) as List<ShopDistanceModel>;

                if (shopDistances == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        result = "内存爆了 请联系 13167226393"
                    };
                }

                var shops = ShopLogic.GetAllShopsFromCache();
                var temp = shops.Where(a => a.IsHot == true && a.IsOnline == true).Take(10);

                foreach (var item in temp)
                {
                    item.Distance = shopDistances.First(a => a.ShopId == item.ShopId).Distance;
                }

                return new
                {
                    jsonStatus = 1,
                    result = temp
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    jsonStatus = 0,
                    result = ex
                }; ;
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
        public object GetNearestDistanceShops(string region, int page = 1, int size = 20)
        {
            if (HttpRuntime.Cache.Get(region) as List<ShopDistanceModel> == null)
            {
                CalDistance(region);
            }

            var shopDistances = HttpRuntime.Cache.Get(region) as List<ShopDistanceModel>;

            if (shopDistances == null)
            {
                return new
                {
                    jsonStatus = 0,
                    result = "内存爆了 请联系 13167226393"
                };
            }

            var shops = ShopLogic.GetAllShopsFromCache();
            var temp = shopDistances.OrderBy(a => a.Distance).Skip((page - 1) * size).Take(size);

            var results = new List<ShopModel>();

            foreach (var item in temp)
            {
                var result = shops.First(a => a.ShopId == item.ShopId);
                result.Distance = item.Distance;
                results.Add(result);
            }

            return new
            {
                jsonStatus = 1,
                result = results
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
        public object GetLowestDiscountRateShops(string region, int page = 1, int size = 20)
        {
            if (HttpRuntime.Cache.Get(region) as List<ShopDistanceModel> == null)
            {
                CalDistance(region);
            }

            var shopDistances = HttpRuntime.Cache.Get(region) as List<ShopDistanceModel>;

            if (shopDistances == null)
            {
                return new
                {
                    jsonStatus = 0,
                    result = "内存爆了 请联系 13167226393"
                };
            }

            var shops = ShopLogic.GetAllShopsFromCache();
            var result = shops.OrderBy(a => a.DiscountRate).Skip((page - 1) * size).Take(size);

            return new
            {
                jsonStatus = 1,
                result = result
            };
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
                var shopResult = HttpRuntime.Cache.Get(region) as List<ShopDistanceModel>;

                if (shopResult != null)
                {
                    return new
                    {
                        jsonStatus = 1,
                        result = "成功获取店铺距离"
                    };
                }

                using (var db = new MeiHiEntities())
                {
                    //加载缓存
                    var shops = ShopLogic.GetAllShopsFromCache();
                    var shopDistances = new List<ShopDistanceModel>();

                    foreach (var item in shops)
                    {
                        shopDistances.Add(new ShopDistanceModel()
                        {
                            ShopId = item.ShopId,
                            Distance = HttpUtils.CalOneShop(region, item.Coordinates)
                        });
                    }

                    HttpRuntime.Cache.Insert(region, shopDistances, null,
                          DateTime.Now.AddSeconds(1200), TimeSpan.Zero);

                    return new
                    {
                        jsonStatus = 1,
                        result = "成功获取店铺距离"
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    jsonStatus = 0,
                    result = ex
                };
            }
        }

        /// <summary>
        /// 店铺的服务详情页
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_ShopServices")]
        [AllowAnonymous]
        public object GetShopServices(long shopId, long userId = 0)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.FirstOrDefault(a => a.ShopId == shopId);
                if (shop != null)
                {
                    bool haveAddedToFavorite = false;

                    if (userId != 0)
                    {
                        haveAddedToFavorite = db.UserFavorites.FirstOrDefault(a => a.ShopId == shopId && a.UserId == userId) != null;
                    }

                    var shopModel = new ShopModel()
                    {
                        Coordinates = shop.Coordinates,
                        DetailAddress = shop.DetailAddress,
                        Contract = shop.Contract,
                        ShopTag = shop.ShopTag,
                        ShopId = shop.ShopId,
                        Title = shop.Title,
                        ShopImageUrl = shop.ShopBrandImages != null
                                        && shop.ShopBrandImages.Count > 0
                                        ? shop.ShopBrandImages.FirstOrDefault().url
                                        : null,
                        ProductBrandCount = shop.ProductBrand.Count,
                        Services = ShopLogic.GetServices(shop.ShopId),
                        HaveAddedToFavorite = haveAddedToFavorite
                    };
                    return new
                    {
                        jsonStatus = 1,
                        result = shopModel
                    };
                }

                return new
                {
                    jsonStatus = 0,
                    result = "获取店铺服务列表失败"
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
                        Contract = shop.Contract,
                        ShopId = shop.ShopId,
                        ProductBrandImages = shop.ProductBrand.Select(a => a.ProductUrl).ToList(),
                        PurchaseNotes = shop.PurchaseNotes,
                        ParentShopId = shop.ParentShopId,//当要调用Show_BranchShops时候传递ParentShopId
                        BranchStoreCount = ShopLogic.GetBranchStoreCount(shop.ParentShopId),//如果查询出来有分店 那么就需要调用 接口Show_BranchShops
                        UserComments = ShopLogic.GetUserCommentsTopFiveByShopId(shop.ShopId)
                    };
                    return new
                    {
                        jsonStatus = 1,
                        result = shopModel
                    };
                }

                return new
                {
                    jsonStatus = 0,
                    result = "获取店铺服务列表失败"
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
            var temp = ShopLogic.GetAllUserCommentsByShopId(shopId, page, size);

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

        /// <summary>
        /// 分店列表
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Show_BranchShops")]
        [AllowAnonymous]
        public object GetBranchShops(long shopId, string region)
        {
            //if (HttpRuntime.Cache.Get(region) as List<ShopDistanceModel> == null)
            //{
            //    CalDistance(region);
            //}

            //var shopDistances = HttpRuntime.Cache.Get(region) as List<ShopDistanceModel>;

            //if (shopDistances == null)
            //{
            //    return new
            //    {
            //        jsonStatus = 0,
            //        result = "内存爆了 请联系 13167226393"
            //    };
            //}

            //var shops = ShopLogic.GetAllShopsFromCache();

            //return new
            //{
            //    jsonStatus = 1,
            //    result = shops.Where(a => a.ParentShopId == shopId)
            //};
            using (var db = new MeiHiEntities())
            {
                var shops = db.Shop.Where(a => a.ParentShopId == shopId);

                if (shops != null && shops.Count() > 0)
                {
                    var result = new List<ShopModel>();

                    foreach (var item in shops)
                    {
                        result.Add(new ShopModel()
                        {
                            Coordinates = item.Coordinates,
                            ShopId = item.ShopId,
                            Title = item.Title,
                            DiscountRate = ShopLogic.GetDiscountRate(item.ShopId),
                            RegionName = item.Region.Name,
                            ShopImageUrl = item.ShopBrandImages.FirstOrDefault() != null ? item.ShopBrandImages.FirstOrDefault().url : "",
                            Rate = ShopLogic.GetShopRate(item.ShopId),
                            ParentShopId = item.ParentShopId,
                            IsHot = item.IsHot,
                            IsOnline = item.IsOnline
                        });
                    }

                    if (HttpRuntime.Cache.Get(region) as List<ShopDistanceModel> == null)
                    {
                        CalDistance(region);
                    }

                    var shopDistances = HttpRuntime.Cache.Get(region) as List<ShopDistanceModel>;

                    foreach (var x in result)
                    {
                        x.Distance = shopDistances.First(a => a.ShopId == x.ShopId).Distance;
                    }

                    return new
                    {
                        jsonStatus = 1,
                        result = result
                    };
                }

                return new
                {
                    jsonStatus = 0,
                    result = "没有分店"
                };
            }
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

                if (shop != null && shop.ShopBrandImages != null && shop.ShopBrandImages.Count > 0)
                {
                    return new
                    {
                        jsonStatus = 1,
                        result = shop.ShopBrandImages.Select(a => a.url)
                    };
                }

                return new
                {
                    jsonStatus = 0,
                    result = "店铺没有品牌图片"
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
                    var favorite = db.UserFavorites.FirstOrDefault(a => a.ShopId == shopId && a.UserId == userId);

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
                        ShopId = shopId,
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
        public object CancelFavorite(long shopId, long userId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var favorite = db.UserFavorites.FirstOrDefault(a => a.ShopId == shopId && a.UserId == userId);

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
