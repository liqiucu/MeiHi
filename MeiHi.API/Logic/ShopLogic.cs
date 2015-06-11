using MeiHi.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using MeiHi.API.Helper;

namespace MeiHi.API.Logic
{
    public static class ShopLogic
    {
        private static readonly object CacheLockObjectGetAllShops = new object();

        public static List<ShopModel> GetAllShopsFromCache()
        {
            using (var db = new MeiHiEntities())
            {
                var shops = HttpRuntime.Cache.Get("AllShops") as List<ShopModel>;

                if (shops == null)
                {
                    lock (CacheLockObjectGetAllShops)
                    {
                        var temp = db.Shop.Where(a => a.IsOnline).ToList();

                        shops = new List<ShopModel>();

                        foreach (var item in temp)
                        {
                            var shopModel = new ShopModel()
                            {
                                Coordinates = item.Coordinates,
                                ShopId = item.ShopId,
                                Title = item.Title,
                                DiscountRate = GetDiscountRate(item.ShopId),
                                RegionName = item.Region.Name,
                                ShopImageUrl = item.ShopBrandImages.FirstOrDefault() != null ? item.ShopBrandImages.FirstOrDefault().url : "",
                                Rate = GetShopRate(item.ShopId),
                                ParentShopId = item.ParentShopId,
                                IsHot = item.IsHot,
                                IsOnline = item.IsOnline
                            };

                            shops.Add(shopModel);
                        }

                        HttpRuntime.Cache.Insert("AllShops", shops, null,
                           DateTime.Now.AddSeconds(1200), TimeSpan.Zero);
                    }
                }

                return shops;
            }
        }

        private static readonly object CacheLockObjectGetUserCommentsByShopId = new object();

        public static List<UserComments> GetUserCommentsByShopId(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var userComments = HttpRuntime.Cache.Get("GetUserCommentsByShopId" + shopId) as List<UserComments>;

                if (userComments == null)
                {
                    lock (CacheLockObjectGetUserCommentsByShopId)
                    {
                        userComments = db.UserComments.Where(a => a.ShopId == shopId && a.Display == true).ToList();
                        HttpRuntime.Cache.Insert("GetUserCommentsByShopId" + shopId, userComments, null,
                           DateTime.Now.AddSeconds(600), TimeSpan.Zero);
                    }
                }

                return userComments;
            }
        }

        //private static readonly object CacheLockObjectGetUserCommentsByServiceId = new object();

        //public static List<UserComments> GetUserCommentsByServiceId(long serviceId)
        //{
        //    using (var db = new MeiHiEntities())
        //    {
        //        var userComments = HttpRuntime.Cache.Get("GetUserCommentsByShopId" + shopId) as List<UserComments>;

        //        if (userComments == null)
        //        {
        //            lock (CacheLockObjectGetUserCommentsByServiceId)
        //            {
        //                userComments = db.UserComments.Where(a => a.ShopId == shopId).ToList();
        //                HttpRuntime.Cache.Insert("GetUserCommentsByShopId" + shopId, userComments, null,
        //                   DateTime.Now.AddSeconds(600), TimeSpan.Zero);
        //            }
        //        }

        //        return userComments;
        //    }
        //}

        public static decimal GetShopRate(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var temp = GetUserCommentsByShopId(shopId);

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

        public static List<UserCommentsModel> GetAllUserCommentsByShopId(long shopId, int page, int size)
        {
            using (var db = new MeiHiEntities())
            {
                var comments = db.UserComments.Where(a => a.ShopId == shopId && a.Display == true).OrderByDescending(a => a.DateCreated).Skip((page - 1) * size).Take(size);

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

        public static List<UserCommentsModel> GetAllUserComments(int page, int size)
        {
            using (var db = new MeiHiEntities())
            {
                var comments = db.UserComments.Where(a => a.Display == true).OrderByDescending(a => a.DateCreated).Skip((page - 1) * size).Take(size);

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

        public static List<UserCommentsModel> GetUserCommentsTopFiveByServiceId(long serviceId)
        {
            using (var db = new MeiHiEntities())
            {
                var comments = db.UserComments.Where(a => a.ServiceId == serviceId && a.Display == true);

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

        public static List<UserCommentsModel> GetAllUserCommentsByServiceId(long serviceId, int page, int size)
        {
            using (var db = new MeiHiEntities())
            {
                var comments = db.UserComments.Where(a => a.ServiceId == serviceId && a.Display == true).OrderByDescending(a => a.DateCreated).Skip((page - 1) * size).Take(size);

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
                var services = GetServicesByShopId(ShopId);
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

        private static readonly object CacheLockObjectGetServicesByShopId = new object();
        public static List<Service> GetServicesByShopId(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var services = HttpRuntime.Cache.Get("AllService" + shopId) as List<Service>;

                if (services == null)
                {
                    lock (CacheLockObjectGetServicesByShopId)
                    {
                        services = db.Service.Where(a => a.ShopId == shopId).ToList();
                        HttpRuntime.Cache.Insert("AllService" + services, services, null,
                           DateTime.Now.AddSeconds(600), TimeSpan.Zero);
                    }

                }

                return services;
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

        //public static List<string> GetProductBrandImages(long shopId)
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
    }
}