using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using MeiHi.Admin.Models;
using PagedList;

namespace MeiHi.Admin.Logic
{
    public class ShopLogic
    {
        MeiHiEntities db = new MeiHiEntities();

        public List<RegionModel> GetRegionsModel()
        {
            List<RegionModel> result = new List<RegionModel>();
            var regions = db.Region.Where(a => a.ProvinceId == 1);

            foreach (var item in regions)
            {
                RegionModel model = new RegionModel()
                {
                    RegionId = item.RegionId,
                    RegionName = item.Name
                };
                result.Add(model);
            }

            return result;
        }

        public long GetParentShopId(string shopName)
        {
            using(var db=new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.Title == shopName).FirstOrDefault();
                if (shop != null)
                {
                    return shop.ShopId;
                }

                return 0;
            }
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

        public StaticPagedList<ShopListDetailModel> GetShops(int page, int pageSize)
        {
            using (var access = new MeiHiEntities())
            {
                var shops = access.Shop.OrderBy(a => a.ShopId).Skip((page - 1) * pageSize).Take(pageSize);
                List<ShopListDetailModel> shopLists = new List<ShopListDetailModel>();
                foreach (var item in shops)
                {
                    ShopListDetailModel temp = new ShopListDetailModel()
                    {
                        Comment = item.Comment,
                        Contract = item.Contract,
                        IsHot = item.IsHot,
                        IsOnline = item.IsOnline,
                        DetailAddress = item.DetailAddress,
                        Phone = item.Phone,
                        RegionName = GetRegionName(item.RegionID, item.ShopId),
                        ShopId = item.ShopId
                    };
                    shopLists.Add(temp);
                }

                StaticPagedList<ShopListDetailModel> result = new StaticPagedList<ShopListDetailModel>
                    (shopLists, page, pageSize, access.Shop.Count());
                return result;
            }
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