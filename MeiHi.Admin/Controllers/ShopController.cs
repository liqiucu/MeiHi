using MeiHi.Admin.Models;
using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PagedList.Mvc;
using PagedList;
using Owin;
using MeiHi.Admin.Logic;
using System.Data.Entity.Validation;
using MeiHi.Admin.Models.Service;
using System.IO;
using MeiHi.Admin.Helper;

namespace MeiHi.Admin.Controllers
{
    public class ShopController : Controller
    {
        /// <summary>
        /// 展示店铺列表 分页
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult ShopManege(int page = 1)
        {
            ShopModel shopmodel = new ShopModel();
            shopmodel.Lists = ShopLogic.GetShops(page, 10);
            return View(shopmodel);
        }

        /// <summary>
        /// 热门首页店铺
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult RecommandShopManege()
        {
            using (var db = new MeiHiEntities())
            {
                List<RecommandShopModel> models = new List<RecommandShopModel>();

                foreach (var item in db.Shop.Where(a => a.IsHot))
                {
                    var shop = db.Shop.FirstOrDefault(a => a.ShopId == item.ShopId);

                    if (shop == null)
                    {
                        throw new Exception("店铺不存在 shopid：" + item.ShopId);
                    }

                    var model = new RecommandShopModel()
                    {
                        LastModifyTime = item.DateModified,
                        ShopId = item.ShopId,
                        Region = shop.Region.Name,
                        ShopName = shop.Title
                    };
                    models.Add(model);
                }

                return View(models);
            }
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult DeleteRecommandShop(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.FirstOrDefault(a => a.ShopId == shopId);

                if (shop == null)
                {
                    throw new Exception("店铺不存在 shopid:" + shopId);
                }

                shop.IsHot = false;
                db.SaveChanges();

                return RedirectToAction("RecommandShopManege");
            }
        }

        /// <summary>
        /// 店铺下架管理
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult OfflineShopManage(int page = 1)
        {
            ShopModel shopmodel = new ShopModel();
            shopmodel.Lists = ShopLogic.GetShops(page, 10, false);
            return View(shopmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult SaveShop(CreateShopMpdel model, HttpPostedFileBase[] ProductBrandFile, HttpPostedFileBase[] shopProductFile)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    string productBrandId = Guid.NewGuid().ToString();

                    foreach (HttpPostedFileBase file in ProductBrandFile)
                    {
                        if (file != null)
                        {
                            string tempUploadPath = "/upload/product/";

                            if (!Directory.Exists(HttpContext.Server.MapPath(tempUploadPath)))
                            {
                                Directory.CreateDirectory(HttpContext.Server.MapPath(tempUploadPath));
                            }

                            string path = System.IO.Path.Combine(HttpContext.Server.MapPath(tempUploadPath), System.IO.Path.GetFileName(file.FileName));
                            file.SaveAs(path);

                            db.ProductBrand.Add(new ProductBrand()
                            {

                                ProductUrl = "http://" + Request.Url.Authority + tempUploadPath + System.IO.Path.GetFileName(file.FileName),
                                ProductBrandId = productBrandId,
                                DateCreated = DateTime.Now,
                                DateModified = DateTime.Now
                            });
                        }
                    }

                    db.SaveChanges();

                    string tempPath = "";
                    string tempUploadShopPath = "/upload/shop/";

                    if (shopProductFile != null && shopProductFile.FirstOrDefault() != null)
                    {
                        if (!Directory.Exists(HttpContext.Server.MapPath(tempUploadShopPath)))
                        {
                            Directory.CreateDirectory(HttpContext.Server.MapPath(tempUploadShopPath));
                        }

                        tempPath = System.IO.Path.Combine(Server.MapPath(tempUploadShopPath), System.IO.Path.GetFileName(shopProductFile[0].FileName));
                    }

                    db.Shop.Add(new Shop()
                    {
                        RegionID = model.RegionId,
                        ShopTag = model.ShopTag,
                        Comment = model.Comment,
                        PurchaseNotes = model.PurchaseNotes,
                        ProductBrandId = productBrandId,
                        Phone = model.Phone,
                        ParentShopId = ShopLogic.GetParentShopId(model.ParentShopName),
                        IsOnline = model.IsOnline,
                        IsHot = model.IsHot,
                        Contract = model.Contract,
                        Coordinates = model.Coordinates,
                        Title = model.Title,
                        ImageUrl = tempPath,
                        DetailAddress = model.DetailAddress,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now
                    });

                    db.SaveChanges();
                    var shopId = ShopLogic.GetShopIdByShopName(model.Title);
                    if (model.IsOnline)
                    {
                        //send username and password to shop then shop can verify the  booking
                        LuoSiMaoTextMessage.SendShopText(model.Phone);
                        db.ShopUser.Add(new ShopUser()
                        {
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now,
                            ShopId = shopId,
                            ShopUserName = model.Phone,
                            Password = model.Phone.Substring(model.Phone.Length - 6)
                        });
                    }

                    foreach (var file in shopProductFile)
                    {
                        if (file != null)
                        {
                            string path = System.IO.Path.Combine(Server.MapPath(tempUploadShopPath), System.IO.Path.GetFileName(file.FileName));

                            file.SaveAs(path);

                            db.ShopBrandImages.Add(new ShopBrandImages()
                            {
                                ShopId = shopId,
                                DateCreated = DateTime.Now,
                                url = "http://" + Request.Url.Authority + tempUploadShopPath + System.IO.Path.GetFileName(file.FileName)
                            });
                        }
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("ShopManege");
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ProductBrandFile">产品</param>
        /// <param name="shopProductFile">店铺</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult UpdateShop(EditShopMpdel model, HttpPostedFileBase[] ProductBrandFile, HttpPostedFileBase[] shopProductFile)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var shop = db.Shop.Where(a => a.ShopId == model.ShopId).FirstOrDefault();
                    if (shop == null)
                    {
                        throw new Exception("the shop not exist shopId:" + model.ShopId);
                    }

                    string productBrandId = shop.ProductBrandId;
                    //ProductBrand 产品
                    if (ProductBrandFile != null && ProductBrandFile.FirstOrDefault() != null)
                    {
                        var productBrands = db.ProductBrand.Where(a => a.ProductBrandId == productBrandId);

                        foreach (var item in productBrands)
                        {
                            db.ProductBrand.Remove(item);
                        }

                        foreach (var file in ProductBrandFile)
                        {
                            string tempUploadPath = "/upload/product/";

                            if (!Directory.Exists(HttpContext.Server.MapPath(tempUploadPath)))
                            {
                                Directory.CreateDirectory(HttpContext.Server.MapPath(tempUploadPath));
                            }
                            string path = System.IO.Path.Combine(Server.MapPath(tempUploadPath), System.IO.Path.GetFileName(file.FileName));
                            file.SaveAs(path);

                            db.ProductBrand.Add(new ProductBrand()
                            {
                                ProductUrl = "http://" + Request.Url.Authority + tempUploadPath + System.IO.Path.GetFileName(file.FileName),
                                ProductBrandId = productBrandId,
                                DateCreated = DateTime.Now,
                                DateModified = DateTime.Now
                            });
                        }

                        db.SaveChanges();
                    }

                    string tempPath = shop.ImageUrl;
                    //ShopBrandImages 店铺
                    if (shopProductFile != null && shopProductFile.FirstOrDefault() != null)
                    {
                        string tempUploadPath = "/upload/shop/";

                        if (!Directory.Exists(HttpContext.Server.MapPath(tempUploadPath)))
                        {
                            Directory.CreateDirectory(HttpContext.Server.MapPath(tempUploadPath));
                        }

                        tempPath = System.IO.Path.Combine(Server.MapPath(tempUploadPath), System.IO.Path.GetFileName(shopProductFile[0].FileName));

                        foreach (var item in shop.ShopBrandImages)
                        {
                            db.ShopBrandImages.Remove(item);
                        }

                        foreach (var file in shopProductFile)
                        {
                            string path = System.IO.Path.Combine(Server.MapPath(tempUploadPath), System.IO.Path.GetFileName(file.FileName));

                            file.SaveAs(path);

                            db.ShopBrandImages.Add(new ShopBrandImages()
                            {
                                ShopId = ShopLogic.GetShopIdByShopName(model.Title),
                                DateCreated = DateTime.Now,
                                url = "http://" + Request.Url.Authority + tempUploadPath + System.IO.Path.GetFileName(file.FileName)
                            });
                        }

                        db.SaveChanges();
                    }

                    shop.RegionID = model.RegionId;
                    shop.ShopTag = model.ShopTag;
                    shop.Comment = model.Comment;
                    shop.PurchaseNotes = model.PurchaseNotes;
                    shop.ProductBrandId = productBrandId;
                    shop.Phone = model.Phone;
                    shop.ParentShopId = ShopLogic.GetParentShopId(model.ParentShopName);
                    shop.IsOnline = model.IsOnline;
                    shop.IsHot = model.IsHot;
                    shop.Contract = model.Contract;
                    shop.Coordinates = model.Coordinates;
                    shop.Title = model.Title;
                    shop.ImageUrl = tempPath;
                    shop.DetailAddress = model.DetailAddress;
                    shop.DateModified = DateTime.Now;

                    //var recommandShop = db.RecommandShop.FirstOrDefault(a => a.RecommandShopId == model.ShopId);

                    //if (model.IsHot && recommandShop == null)
                    //{
                    //    db.RecommandShop.Add(new RecommandShop()
                    //    {
                    //        DateCreated = DateTime.Now,
                    //        DateModified = DateTime.Now,
                    //        RecommandShopId = model.ShopId
                    //    });
                    //}

                    //if (!model.IsHot && recommandShop != null)
                    //{
                    //    db.RecommandShop.Remove(shop.RecommandShop);
                    //}

                    db.SaveChanges();
                }

                return RedirectToAction("ShopManege");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult CreateShop()
        {
            CreateShopMpdel model = new CreateShopMpdel()
            {
                RegionNameList = new CommonLogic().RegionList()
            };

            return View(model);
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult EditShop(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                if (shop != null)
                {
                    EditShopMpdel model = new EditShopMpdel()
                    {
                        RegionNameList = new CommonLogic().RegionList(shop.RegionID),
                        Comment = shop.Comment,
                        ShopId = shopId,
                        Contract = shop.Contract,
                        Phone = shop.Phone,
                        Coordinates = shop.Coordinates,
                        DetailAddress = shop.DetailAddress,
                        IsHot = shop.IsHot,
                        IsOnline = shop.IsOnline,
                        PurchaseNotes = shop.PurchaseNotes,
                        Title = shop.Title,
                        ShopTag = shop.ShopTag,
                        ParentShopName = ShopLogic.GetShopNameByShopId(shop.ParentShopId.Value),
                        ProductBrandList = ShopLogic.GetProductBrandImages(shop.ProductBrandId),
                        ShopProductList = shop.ShopBrandImages != null
                                            && shop.ShopBrandImages.Count > 0
                                            ? shop.ShopBrandImages.Select(a => a.url).ToList()
                                            : null
                    };

                    return View(model);
                }

                return View();
            }
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult ShopDetail(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                if (shop != null)
                {
                    ShopDetailMpdel model = new ShopDetailMpdel()
                    {
                        ShopId = shopId,
                        Comment = shop.Comment,
                        Contract = shop.Contract,
                        Phone = shop.Phone,
                        Coordinates = shop.Coordinates,
                        DetailAddress = shop.DetailAddress,
                        IsHot = shop.IsHot,
                        IsOnline = shop.IsOnline,
                        PurchaseNotes = shop.PurchaseNotes,
                        Title = shop.Title,
                        ShopTag = shop.ShopTag,
                        ParentShopName = ShopLogic.GetShopNameByShopId(shop.ParentShopId.Value),
                        RegionName = shop.Region.Name,
                        ProductBrandList = ShopLogic.GetProductBrandImages(shop.ProductBrandId),
                        ShopProductList = shop.ShopBrandImages != null
                                            && shop.ShopBrandImages.Count > 0
                                            ? shop.ShopBrandImages.Select(a => a.url).ToList()
                                            : null
                    };

                    return View(model);
                }

                return View();
            }
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult DeleteShop(long shopId)
        {
            using (var access = new MeiHiEntities())
            {
                var shop = access.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                if (shop != null)
                {
                    access.Booking.RemoveRange(shop.Booking);

                    if (shop.RecommandShop != null)
                    {
                        access.RecommandShop.Remove(shop.RecommandShop);
                    }

                    access.Service.RemoveRange(shop.Service);
                    access.ShopBrandImages.RemoveRange(shop.ShopBrandImages);
                    access.ShopUser.RemoveRange(shop.ShopUser);
                    access.ProductBrand.RemoveRange(access.ProductBrand.Where(a => a.ProductBrandId == shop.ProductBrandId));
                    access.UserComments.RemoveRange(shop.UserComments);
                    access.UserFavorites.RemoveRange(shop.UserFavorites);
                    access.Shop.Remove(shop);
                    access.SaveChanges();
                }
            }

            return RedirectToAction("ShopManege");
        }

        #region service
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult SaveService(CreateServiceModel model, HttpPostedFileBase[] serviceTitleUrl)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var service = db.Service.Where(a => a.ServiceId == model.ServiceId).FirstOrDefault();
                    string tempUploadServicePath = "/upload/service/";

                    if (!Directory.Exists(HttpContext.Server.MapPath(tempUploadServicePath)))
                    {
                        Directory.CreateDirectory(HttpContext.Server.MapPath(tempUploadServicePath));
                    }

                    if (service == null)
                    {
                        service = new Service();
                        foreach (var file in serviceTitleUrl)
                        {
                            if (file != null)
                            {
                                string path = System.IO.Path.Combine(Server.MapPath(tempUploadServicePath), System.IO.Path.GetFileName(file.FileName));
                                file.SaveAs(path);
                                service.TitleUrl = "http://" + Request.Url.Authority + tempUploadServicePath + System.IO.Path.GetFileName(file.FileName);
                            }
                        }
                        service.ServiceTypeId = model.ServiceTypeId;
                        service.CMUnitCost = model.CMUnitCost;
                        service.Designer = model.Designer;
                        service.Detail = model.Detail;
                        service.OriginalUnitCost = model.OriginalUnitCost;
                        service.Title = model.Title;
                        service.ShopId = model.ShopId;
                        service.IfSupportRealTimeRefund = model.IfSupportRealTimeRefund;
                        service.DateCreated = DateTime.Now;
                        service.DateModified = DateTime.Now;
                        db.Service.Add(service);
                    }
                    else
                    {
                        foreach (var file in serviceTitleUrl)
                        {
                            if (file != null)
                            {
                                string path = System.IO.Path.Combine(Server.MapPath(tempUploadServicePath), System.IO.Path.GetFileName(file.FileName));
                                file.SaveAs(path);
                                service.TitleUrl = "http://" + Request.Url.Authority + tempUploadServicePath + System.IO.Path.GetFileName(file.FileName);
                            }
                        }
                        service.ServiceTypeId = model.ServiceTypeId;
                        service.CMUnitCost = model.CMUnitCost;
                        service.Designer = model.Designer;
                        service.Detail = model.Detail;
                        service.OriginalUnitCost = model.OriginalUnitCost;
                        service.Title = model.Title;
                        service.IfSupportRealTimeRefund = model.IfSupportRealTimeRefund;
                        service.DateModified = DateTime.Now;
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("ShowServicesByShopId", new { shopId = model.ShopId });
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult CreateService(long shopId)
        {
            using (var access = new MeiHiEntities())
            {
                //var shop = access.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                CreateServiceModel model = new CreateServiceModel()
                {
                    ShopId = shopId,
                    ServiceTypeLists = ServiceLogic.ServiceTypeList()
                };
                return View(model);
            }
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult ShowServicesByShopId(long shopId)
        {
            using (var access = new MeiHiEntities())
            {
                List<ShowServiceListModel> results = new List<ShowServiceListModel>();

                var services = access.Service.Where(a => a.ShopId == shopId);

                if (services != null && services.Count() > 0)
                {

                    foreach (var item in services)
                    {
                        results.Add(new ShowServiceListModel()
                        {
                            ServiceId = item.ServiceId,
                            CMUnitCost = item.CMUnitCost,
                            ServiceTypeName = item.ServiceType.Title,
                            Title = item.Title,
                            OriginalUnitCost = item.OriginalUnitCost,
                            IfSupportRealTimeRefund = item.IfSupportRealTimeRefund
                        });
                    }
                }

                ShowServiceModel model = new ShowServiceModel();
                model.ShopId = shopId;
                model.ShowServiceList = results;
                return View(model);
            }
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult ServiceDetail(long serviceId)
        {
            using (var db = new MeiHiEntities())
            {
                var service = db.Service.Where(a => a.ServiceId == serviceId).FirstOrDefault();

                if (service != null)
                {
                    ServiceDetailModel model = new ServiceDetailModel()
                    {
                        ShopId = service.ShopId,
                        ServiceId = service.ServiceId,
                        CMUnitCost = service.CMUnitCost,
                        Designer = service.Designer,
                        Detail = service.Detail,
                        IfSupportRealTimeRefund = service.IfSupportRealTimeRefund,
                        OriginalUnitCost = service.OriginalUnitCost,
                        //PurchaseNotes = service.Shop.PurchaseNotes,
                        ServiceTypeName = service.ServiceType.Title,
                        Title = service.Title,
                        TitleUrl = service.TitleUrl
                    };

                    return View(model);
                }

                return View();
            }
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult EditService(long serviceId)
        {
            using (var db = new MeiHiEntities())
            {
                var service = db.Service.Where(a => a.ServiceId == serviceId).FirstOrDefault();

                if (service != null)
                {
                    var model = new CreateServiceModel()
                    {
                        ShopId = service.ShopId,
                        ServiceId = service.ServiceId,
                        Title = service.Title,
                        TitleUrl = service.TitleUrl,
                        CMUnitCost = service.CMUnitCost,
                        Designer = service.Designer,
                        Detail = service.Detail,
                        IfSupportRealTimeRefund = service.IfSupportRealTimeRefund,
                        OriginalUnitCost = service.OriginalUnitCost,
                        //PurchaseNotes = service.Shop.PurchaseNotes,
                        ServiceTypeLists = ServiceLogic.ServiceTypeList(service.ServiceTypeId)
                    };

                    return View(model);
                }

                return View();
            }
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult DeleteService(long serviceId)
        {
            using (var access = new MeiHiEntities())
            {
                var service = access.Service.Where(a => a.ServiceId == serviceId).FirstOrDefault();

                if (service != null)
                {
                    access.UserComments.RemoveRange(service.UserComments);
                    access.UserFavorites.RemoveRange(service.UserFavorites);
                    access.Booking.RemoveRange(service.Booking);
                    access.Service.Remove(service);
                    access.SaveChanges();
                    return RedirectToAction("ShowServicesByShopId", new { shopId = service.ShopId });
                }

                return RedirectToAction("ShopManege");
            }
        }
        #endregion
    }
}