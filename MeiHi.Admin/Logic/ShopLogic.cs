using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using MeiHi.Admin.Models;
using PagedList;
using MeiHi.Admin.Models.UserComments;

namespace MeiHi.Admin.Logic
{
    public static class ShopLogic
    {
        //public static long CreateShop()
        //{
        //    db.Shop.Add(new Shop()
        //    {
        //        RegionID = model.RegionId,
        //        ShopTag = model.ShopTag,
        //        Comment = model.Comment,
        //        PurchaseNotes = model.PurchaseNotes,
        //        ProductBrandId = productBrandId,
        //        Phone = model.Phone,
        //        ParentShopId = ShopLogic.GetParentShopId(model.ParentShopName),
        //        IsOnline = model.IsOnline,
        //        IsHot = model.IsHot,
        //        Contract = model.Contract,
        //        Coordinates = model.Coordinates,
        //        Title = model.Title,
        //        ImageUrl = imageTitleUrl,
        //        DetailAddress = model.DetailAddress,
        //        DateCreated = DateTime.Now,
        //        DateModified = DateTime.Now
        //    });
        //}

        //public static bool DeleteProductBrand(string productBrandId)
        //{
        //    using(var db=new MeiHiEntities())
        //    {
        //        var productBrands=db.ProductBrand.Where(a=>a.ProductBrandId==productBrandId)
        //    }
        //}


        public static bool HaveRegisteredShopMobile(string mobile)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.FirstOrDefault(a => a.Phone == mobile);

                if (shop == null)
                {
                    return false;
                }

                return true;
            }
        }

        public static bool CheckParentShopName(string shopName)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.FirstOrDefault(a => a.Title == shopName);

                if (shop == null)
                {
                    return false;
                }

                return true;
            }
        }

        public static bool HaveRegisteredShopName(string shopName)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.FirstOrDefault(a => a.Title == shopName);

                if (shop == null)
                {
                    return false;
                }

                return true;
            }
        }

        public static StaticPagedList<ShopListDetailModel> GetShops(int page, int pageSize, bool isOnline = true)
        {
            using (var access = new MeiHiEntities())
            {
                var shops = access.Shop.Where(a => a.IsOnline == isOnline).OrderByDescending(a => a.IsHot).Skip((page - 1) * pageSize).Take(pageSize);
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
                        ShopId = item.ShopId,
                        Title = item.Title
                    };
                    shopLists.Add(temp);
                }

                StaticPagedList<ShopListDetailModel> result = new StaticPagedList<ShopListDetailModel>
                    (shopLists, page, pageSize, access.Shop.Count());
                return result;
            }
        }

        public static long GetShopIdByShopName(string shopName)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.Title == shopName).FirstOrDefault();
                if (shop != null)
                {
                    return shop.ShopId;
                }

                return 0;
            }
        }

        public static string GetShopNameByShopId(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();
                if (shop != null)
                {
                    return shop.Title;
                }

                return "";
            }
        }

        public static long GetParentShopId(string shopName)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.Title == shopName).FirstOrDefault();
                if (shop != null)
                {
                    return shop.ShopId;
                }

                return 0;
            }
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

        //public static string GetShopImageUrl(string ProductBrandId)
        //{
        //    using (var db = new MeiHiEntities())
        //    {
        //        if (string.IsNullOrEmpty(ProductBrandId))
        //        {
        //            return "";
        //        }
        //        var temp = db.ProductBrand.Where(a => a.ProductBrandId == ProductBrandId).FirstOrDefault();
        //        if (temp != null)
        //        {
        //            return temp.ProductUrl;
        //        }
        //        return "";
        //    }
        //}

        /// <summary>
        /// 品牌照片数量
        /// </summary>
        /// <param name="ProductBrandId"></param>
        /// <returns></returns>
        //public static int GetProductBrandCount(string ProductBrandId)
        //{
        //    using (var db = new MeiHiEntities())
        //    {
        //        if (string.IsNullOrEmpty(ProductBrandId))
        //        {
        //            return 0;
        //        }

        //        var temp = db.ProductBrand.Where(a => a.ProductBrandId == ProductBrandId);

        //        if (temp != null && temp.Count() > 0)
        //        {
        //            return temp.Count();
        //        }

        //        return 0;
        //    }
        //}

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

        /// <summary>
        /// 店铺详情页 需要的用户评论
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static List<UserCommentsModel> GetUserCommentsTopFiveByShopId(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var comments = db.UserComments.Where(a => a.ShopId == shopId && a.Display == true);

                if (comments != null && comments.Count() > 0)
                {
                    List<UserCommentsModel> result = new List<UserCommentsModel>();

                    foreach (var item in comments.Take(5))
                    {
                        var temp = item.UserCommentSharedImg.Where(a => a.UserCommentId == item.UserCommentId);
                        var temp1 = item.UserCommentsReply.Where(a => a.UserCommentId == item.UserCommentId);

                        result.Add(new UserCommentsModel()
                        {
                            Comment = item.Comment,
                            DateCreated = item.DateCreated,
                            Rate = item.Rate,
                            ServiceName = item.ServiceName,
                            ShopId = item.ShopId,
                            UserFullName = item.User.FullName,
                            UserSharedImgaeList = temp != null && temp.Count() > 0 ? temp.Select(a => a.ImgUrl).ToList() : null,
                            MeiHiReply = temp1 != null && temp1.Count() > 0 ? temp1.Select(a => a.Comment).ToList() : null,
                        });
                    }

                    return result;
                }

                return null;
            }
        }

        public static List<UserCommentsModel> GetAllUserCommentsByShopId(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var comments = db.UserComments.Where(a => a.ShopId == shopId && a.Display == true);

                if (comments != null && comments.Count() > 0)
                {
                    List<UserCommentsModel> result = new List<UserCommentsModel>();

                    foreach (var item in comments)
                    {
                        var temp = item.UserCommentSharedImg.Where(a => a.UserCommentId == item.UserCommentId);
                        var temp1 = item.UserCommentsReply.Where(a => a.UserCommentId == item.UserCommentId);

                        result.Add(new UserCommentsModel()
                        {
                            Comment = item.Comment,
                            DateCreated = item.DateCreated,
                            Rate = item.Rate,
                            ServiceName = item.ServiceName,
                            ShopId = item.ShopId,
                            UserFullName = item.User.FullName,
                            UserSharedImgaeList = temp != null && temp.Count() > 0 ? temp.Select(a => a.ImgUrl).ToList() : null,
                            MeiHiReply = temp1 != null && temp1.Count() > 0 ? temp1.Select(a => a.Comment).ToList() : null,
                        });
                    }

                    return result;
                }

                return null;
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

        //public static List<string> GetShopProductBrandImages(long shopId)
        //{
        //    using (var db = new MeiHiEntities())
        //    {
        //        var shop = db.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

        //        if (shop != null)
        //        {
        //            if (!string.IsNullOrEmpty(shop.ProductBrandId))
        //            {
        //                var lists = db.ProductBrand.Where(a => a.ProductBrandId == shop.ProductBrandId);

        //                if (lists != null && lists.Count() > 0)
        //                {
        //                    return lists.Select(a => a.ProductUrl).ToList();
        //                }
        //            }
        //        }

        //        return null;
        //    }
        //}

        //public static List<string> GetProductBrandImages(string productBrandId)
        //{
        //    using (var db = new MeiHiEntities())
        //    {
        //        if (!string.IsNullOrEmpty(productBrandId))
        //        {
        //            var lists = db.ProductBrand.Where(a => a.ProductBrandId == productBrandId);

        //            if (lists != null && lists.Count() > 0)
        //            {
        //                return lists.Select(a => a.ProductUrl).ToList();
        //            }
        //        }

        //        return null;
        //    }
        //}
    }
}