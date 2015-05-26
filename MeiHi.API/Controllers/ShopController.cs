﻿using System;
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
            using (var db = new MeiHiEntities())
            {
                var shops = ShopLogic.GetAllShopsFromCache();
                var recommandShops = shops.Where(a => a.IsHot == true && a.IsOnline == true).Take(10);

                if (recommandShops == null || recommandShops.Count() == 0)
                {
                    return new
                    {
                        jsonStatus = 0,
                        result = "没有推荐店铺"
                    };
                }

                return new
                {
                    jsonStatus = 1,
                    resut = recommandShops
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
        public object GetNearestDistanceShops(string region, int page = 1, int size = 20)
        {
            if (System.Web.HttpContext.Current.Session["ShopsDistance"] == null)
            {
                CalDistance(region);
            }

            var shops = ShopLogic.GetAllShopsFromCache();
            var shopDistances = System.Web.HttpContext.Current.Session["ShopsDistance"] as List<ShopDistanceModel>;
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
                resut = results
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
            if (System.Web.HttpContext.Current.Session["ShopsDistance"] == null)
            {
                CalDistance(region);
            }

            var shops = ShopLogic.GetAllShopsFromCache();
            var shopDistances = System.Web.HttpContext.Current.Session["ShopsDistance"] as List<ShopDistanceModel>;
            var result = shops.OrderBy(a => a.DiscountRate).Skip((page - 1) * size).Take(size);

            foreach (var item in result)
            {
                item.Distance = shopDistances.First(a => a.ShopId == item.ShopId).Distance;
            }

            return new
            {
                jsonStatus = 1,
                resut = result
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
                if (System.Web.HttpContext.Current.Session["ShopsDistance"] != null)
                {
                    return new
                    {
                        jsonStatus = 1,
                        resut = "成功获取店铺距离"
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

                    //每个用户的距离信息  放入临时session
                    System.Web.HttpContext.Current.Session["ShopsDistance"] = shopDistances;

                    return new
                    {
                        jsonStatus = 1,
                        resut = "成功获取店铺距离"
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
        public object GetShopServices(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.FirstOrDefault(a => a.ShopId == shopId);
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
                        ShopImageUrl = shop.ShopBrandImages != null
                                        && shop.ShopBrandImages.Count > 0
                                        ? shop.ShopBrandImages.FirstOrDefault().url
                                        : null,
                        ProductBrandCount = shop.ProductBrand.Count,
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
                        ProductBrandImages = shop.ProductBrand.Select(a => a.ProductUrl).ToList(),
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
            var temp = ShopLogic.GetAllUserCommentsByShopId(shopId, page, size);

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

                if (shop != null && shop.ShopBrandImages != null && shop.ShopBrandImages.Count > 0)
                {
                    return new
                    {
                        jsonStatus = 1,
                        resut = shop.ShopBrandImages.Select(a => a.url)
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
    }
}
