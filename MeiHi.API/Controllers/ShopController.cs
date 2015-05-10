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
            int index = 0;
            var shopResults = new List<ShopModel>();
            int count = shops.Count() < 5 ? shops.Count() : 5;
            var temp = new List<ShopModel>(count);

            for (int j = 0; j < shops.Count(); j++)
            {
                if (index != count)
                {
                    temp.Add(ConvertToShopModel(shops[j]));
                    index++;
                }
                if (index == count)
                {
                    var result = HttpUtils.RequestApi(
                        origin: region,
                        destinations: temp.Select(a => a.DetailAddress).ToList());

                    var addressShopError = result.Result.Elements.Any(a => a.Distance == null || a.Duration == null);

                    if (result == null
                        || result.Status != 0
                        || addressShopError)
                    {
                        throw new Exception("店铺地址不对 shop ID列表：" + string.Join(",", temp.Select(a => a.ShopId).ToList()));
                    }

                    for (int i = 0; i < count; i++)
                    {
                        temp[i].Distance = result.Result.Elements[i].Distance.Value;
                    }

                    shopResults.AddRange(temp);
                    count = shops.Count - j < 5 ? shops.Count - j : 5;
                    temp = new List<ShopModel>(count);
                    index = 0;
                }
            }
            System.Web.HttpContext.Current.Session["Shops"] = shopResults;
        }

        private ShopModel ConvertToShopModel(Shop shop)
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
                ShopImageUrl = GetShopImageUrl(shop.ProductBrandId)
            };
        }

        private string GetShopImageUrl(long? ProductBrandId)
        {
            if (ProductBrandId == null)
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

        private int GetBranchStoreCount(long? ParentShopId)
        {
            if (ParentShopId == null)
            {
                return 0;
            }
            return db.Shop.Where(a => a.ParentShopId == ParentShopId).Count();
        }

        private decimal GetDiscountRate(long ShopId)
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

        private string GetRegionName(int regionId, long shopId)
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
