using MeiHi.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using MeiHi.API.Helper;
using MeiHi.API.Models.UserComments;

namespace MeiHi.API.Logic
{
    public static class ShopLogic
    {

        public static ShopModel ConvertToShopModel(Shop shop, string startCoordinates = "")
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

        public static decimal GetShopRate(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var temp = db.UserComments.Where(a => a.ShopId == shopId);

                if (temp != null && temp.Count() > 0)
                {
                    return decimal.Round(temp.Sum(a => a.Rate) / temp.Count(), 1);
                }

                return 4.5M;
            }
        }

        public static string GetShopImageUrl(string ProductBrandId)
        {
            using (var db = new MeiHiEntities())
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
        }

        /// <summary>
        /// 品牌照片数量
        /// </summary>
        /// <param name="ProductBrandId"></param>
        /// <returns></returns>
        public static int GetProductBrandCount(string ProductBrandId)
        {
            using (var db = new MeiHiEntities())
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
        }

        /// <summary>
        /// 每个店铺的服务列表，按照服务类型名称分组
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static IEnumerable<IGrouping<string, ServiceForShopDetailModel>> GetServices(long shopId)
        {
            using (var db = new MeiHiEntities())
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
        }
        /// <summary>
        /// 分店数量
        /// </summary>
        /// <param name="ParentShopId"></param>
        /// <returns></returns>
        public static int GetBranchStoreCount(long? ParentShopId)
        {
            using (var db = new MeiHiEntities())
            {
                if (ParentShopId == null)
                {
                    return 0;
                }
                return db.Shop.Where(a => a.ParentShopId == ParentShopId).Count();
            }
        }

        public static List<UserCommentsModel> GetUserComments(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var comments = db.UserComments.Where(a => a.ShopId == shopId && a.Display == true);

                if (comments != null && comments.Count() > 0)
                {
                    List<UserCommentsModel> result = new List<UserCommentsModel>();

                    foreach (var item in comments.Take(5))
                    {
                        result.Add(new UserCommentsModel()
                        {
                            Comment = item.Comment,
                            DateCreated = item.DateCreated,
                            Rate = item.Rate,
                            ServiceName = item.ServiceName,
                            ShopId = item.ShopId,
                            ServiceId = item.ServiceId,
                            UserFullName = item.User.FullName,
                            //UserSharedImgaeList= db.UserCommentSharedImg.Where(a=>a.UserCommentSharedImgId==item.UserCommentSharedImg.UserCommentSharedImgId)
                        });
                    }
                }
            }
        }

        public static decimal GetDiscountRate(long ShopId)
        {
            using (var db = new MeiHiEntities())
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
        }


        public static string GetRegionName(int regionId, long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var region = (from a in db.Region where a.RegionId == regionId select a).FirstOrDefault();

                if (region == null)
                {
                    throw new Exception("店铺ID: " + shopId + " 区域未设置");
                }

                return region.Name;
            }
        }

        public static List<string> GetShopProductBrandImages(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                if (shop != null)
                {
                    if (!string.IsNullOrEmpty(shop.ProductBrandId))
                    {
                        var lists = db.ProductBrand.Where(a => a.ProductBrandId == shop.ProductBrandId);

                        if (lists != null && lists.Count() > 0)
                        {
                            return lists.Select(a => a.ProductUrl).ToList();
                        }
                    }
                }

                return null;
            }
        }
    }
}