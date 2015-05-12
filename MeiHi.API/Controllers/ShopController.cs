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

namespace MeiHi.API.Controllers
{
    [RoutePrefix("shop")]
    public class ShopController : ApiController
    {
        private MeiHiEntities db = new MeiHiEntities();

        [HttpGet]
        [Route("recmmondShops")]
        [AllowAnonymous]
        public object GetRecmmondShoppings()
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
                var shopModel = ConvertToShopModel(item);
                results.Add(shopModel);
            }

            return new
            {
                jsonStatus = 1,
                resut = results
            };
        }

        /// <summary>
        /// 默认显示20个店铺,拉一次继续显示20个
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
        /// 默认显示20个店铺,拉一次继续显示20个
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

        //[HttpGet]
        //[Route("Show_ShopDetail")]
        //[AllowAnonymous]
        //public object GetShopDetail(long shopId)
        //{

        //}

        /// <summary>
        /// 计算所有店铺距离当前用户距离
        /// </summary>
        /// <param name="region"></param>
        [AllowAnonymous]
        [Route("caldistance")]
        [HttpPost]
        public void CalDistance(string region)
        {
            var shops = db.Shop.Where(a => a.IsOnline == true).ToList();
            var shopResults = new List<ShopModel>();

            for (int j = 0; j < shops.Count(); j++)
            {
                shopResults.Add(ConvertToShopModel(shops[j], region));
            }

            System.Web.HttpContext.Current.Session["Shops"] = shopResults;
        }

        public ShopModel ConvertToShopModel(Shop shop, string startCoordinates = "")
        {
            return new ShopModel()
            {
                Contract = shop.Contract,
                Coordinates = shop.Coordinates,
                DetailAddress = shop.DetailAddress,
                Phone = shop.Phone,
                PurchaseNotes = shop.PurchaseNotes,
                ShopTag = shop.ShopTag,
                ShopId = shop.ShopId,
                DiscountRate = GetDiscountRate(shop.ShopId),
                BranchStoreCount = GetBranchStoreCount(shop.ParentShopId),
                RegionName = GetRegionName(shop.RegionID, shop.ShopId),
                ShopImageUrl = GetShopImageUrl(shop.ProductBrandId),
                Rate = GetShopRate(shop.ShopId),
                ProductBrandCount = GetProductBrandCount(shop.ProductBrandId),
                Distance = string.IsNullOrEmpty(startCoordinates) ? 0 : HttpUtils.CalOneShop(startCoordinates, shop.Coordinates),
                Services = GetServices(shop.ShopId)
            };
        }

        public decimal GetShopRate(long shopId)
        {
            var temp = db.UserComments.Where(a => a.ShopId == shopId);

            if (temp != null && temp.Count() > 0)
            {
                return decimal.Round(temp.Sum(a => a.Rate) / temp.Count(), 1);
            }

            return 4.5M;
        }

        public string GetShopImageUrl(string ProductBrandId)
        {

            if (string.IsNullOrEmpty(ProductBrandId))
            {
                return "";
            }
            var temp = db.ProductBrand.Where(a => a.ProductBrandId == ProductBrandId).FirstOrDefault();
            if (temp != null)
            {
                return temp.ProductUrl;
            }
            return "";
        }

        /// <summary>
        /// 品牌照片数量
        /// </summary>
        /// <param name="ProductBrandId"></param>
        /// <returns></returns>
        public int GetProductBrandCount(string ProductBrandId)
        {
            if (string.IsNullOrEmpty(ProductBrandId))
            {
                return 0;
            }

            var temp = db.ProductBrand.Where(a => a.ProductBrandId == ProductBrandId);

            if (temp != null && temp.Count() > 0)
            {
                return temp.Count();
            }

            return 0;
        }

        /// <summary>
        /// 每个店铺的服务列表，按照服务类型名称分组
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public IEnumerable<IGrouping<string, ServiceForShopDetailModel>> GetServices(long shopId)
        {
            var services = from a in db.Service
                           where a.ShopId == shopId
                           select a;

            var result = new List<ServiceForShopDetailModel>();

            foreach (var item in services)
            {
                var temp = new ServiceForShopDetailModel
                {
                    CMUnitCost = item.CMUnitCost,
                    OriginalUnitCost = item.OriginalUnitCost,
                    ServiceId = item.ServiceId,
                    Title = item.Title,
                    ShopId = item.ShopId,
                    ServiceTypeId = item.ServiceTypeId
                };
                result.Add(temp);
            }

            var serviceGroups = result.GroupBy(a => a.ServiceTypeName);
            return serviceGroups;
        }
        /// <summary>
        /// 分店数量
        /// </summary>
        /// <param name="ParentShopId"></param>
        /// <returns></returns>
        public int GetBranchStoreCount(long? ParentShopId)
        {
            if (ParentShopId == null)
            {
                return 0;
            }
            return db.Shop.Where(a => a.ParentShopId == ParentShopId).Count();
        }

        public decimal GetDiscountRate(long ShopId)
        {
            var services = db.Service.Where(a => a.ShopId == ShopId);
            if (services != null && services.Count() > 0)
            {
                var lowestDiscountRate = services.OrderBy(a => a.CMUnitCost / a.OriginalUnitCost).FirstOrDefault();

                if (lowestDiscountRate != null)
                {
                    var temp = (lowestDiscountRate.CMUnitCost / lowestDiscountRate.OriginalUnitCost);
                    temp = temp * 10;
                    return decimal.Round(temp, 1);
                }
            }
            return 10;
        }


        public string GetRegionName(int regionId, long shopId)
        {

            var region = (from a in db.Region where a.RegionId == regionId select a).FirstOrDefault();

            if (region == null)
            {
                throw new Exception("店铺ID: " + shopId + " 区域未设置");
            }

            return region.Name;
        }
    }
}
