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
using MeiHi.CommonDll.Helper;
using MeiHi.Admin.Models.City;

namespace MeiHi.Admin.Controllers
{
    public class ShopController : Controller
    {
        #region shop
        /// <summary>
        /// 展示店铺列表 分页
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult ShopManege(int page = 1, string shopName = "", long parentShopId = 0)
        {
            ShopModel shopmodel = new ShopModel();
            shopmodel.Lists = ShopLogic.GetShops(page, 10, true, shopName, parentShopId);

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
        public ActionResult OfflineShopManage(int page = 1, string shopName = "")
        {
            ShopModel shopmodel = new ShopModel();
            shopmodel.Lists = ShopLogic.GetShops(page, 10, false, shopName);
            return View(shopmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult CreateShop(
            CreateShopMpdel model,
            HttpPostedFileBase[] ProductBrandFile,
            HttpPostedFileBase[] shopProductFile)
        {
            var RegionNameList = new CommonLogic().RegionList();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "请检查输入的信息是否有问题");
                model.RegionNameList = RegionNameList;
                model.StreetNameList = new CommonLogic().StreetList(Convert.ToInt32(RegionNameList[0].Value));
                return View(model);
            }

            if (ShopLogic.HaveRegisteredShopMobile(model.Phone))
            {
                ModelState.AddModelError("", "手机已经被注册");
                model.RegionNameList = RegionNameList;
                model.StreetNameList = new CommonLogic().StreetList(Convert.ToInt32(RegionNameList[0].Value));
                return View(model);
            }

            if (ShopLogic.HaveRegisteredShopName(model.Title))
            {
                ModelState.AddModelError("", "店铺名已经被注册");
                model.RegionNameList = RegionNameList;
                model.StreetNameList = new CommonLogic().StreetList(Convert.ToInt32(RegionNameList[0].Value));
                return View(model);
            }

            if (model.ParentShopId > 0 && !ShopLogic.CheckParentShopId(model.ParentShopId))
            {
                ModelState.AddModelError("", "父店铺不合法");
                model.RegionNameList = RegionNameList;
                model.StreetNameList = new CommonLogic().StreetList(Convert.ToInt32(RegionNameList[0].Value));
                return View(model);
            }

            try
            {
                using (var db = new MeiHiEntities())
                {
                    var shop = new Shop()
                    {
                        RegionID = model.RegionId,
                        ShopTag = model.ShopTag,
                        Comment = model.Comment,
                        PurchaseNotes = model.PurchaseNotes,
                        Phone = model.Phone,
                        ParentShopId = model.ParentShopId,
                        IsOnline = model.IsOnline,
                        IsHot = model.IsHot,
                        Contract = model.Contract,
                        Coordinates = model.Coordinates,
                        Title = model.Title,
                        DetailAddress = model.DetailAddress,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now
                    };

                    if (model.StreetId > 0)
                    {
                        shop.StreetId = model.StreetId;
                    }

                    shop.ShopUser.Add(new ShopUser()
                    {
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        ShopUserName = model.Phone,
                        AliPayAccount = model.AliPayAccount,
                        FullName = model.FullName,
                        WeiXinPayAccount = model.WinXinPayAccount,
                        Password = model.Phone.Substring(model.Phone.Length - 6),
                        BankName = model.BankName,
                        BankNo = model.BankNo
                    });

                    var productBrandImages = ImageHelper.SaveImage(
                         Request.Url.Authority,
                         "/upload/shops/" + model.Title + "/product/",
                         ProductBrandFile);

                    foreach (var file in productBrandImages)
                    {
                        shop.ProductBrand.Add(new ProductBrand()
                        {
                            ProductUrl = file,
                            DateCreated = DateTime.Now
                        });
                    }

                    var shopBrandImages = ImageHelper.SaveImage(
                          Request.Url.Authority,
                          "/upload/shops/" + model.Title + "/shop/",
                          shopProductFile);

                    foreach (var file in shopBrandImages)
                    {
                        shop.ShopBrandImages.Add(new ShopBrandImages()
                        {
                            url = file,
                            DateCreated = DateTime.Now
                        });
                    }

                    db.Shop.Add(shop);
                    db.SaveChanges();

                    LuoSiMaoTextMessage.SendShopText(model.Phone, "");
                }

                return RedirectToAction("ShopManege");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetStreetsByRegionId(int regionId)
        {
            using (var db = new MeiHiEntities())
            {
                var result = new List<StreetModel>();
                var streets = db.Region.Where(a => a.ParentRegionId == regionId);

                foreach (var itemStreet in streets)
                {
                    result.Add(new StreetModel()
                    {
                        RegionId = itemStreet.RegionId,
                        RegionName = itemStreet.Name
                    });
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult CreateShop(long shopId = 0)
        {
            long parentShopId = 0;

            if (shopId > 0)
            {
                using (var db = new MeiHiEntities())
                {
                    var shop = db.Shop.First(a => a.ShopId == shopId);

                    if (shop.ParentShopId == null || shop.ParentShopId == 0)
                    {
                        parentShopId = shop.ShopId;
                    }
                    else
                    {
                        parentShopId = shop.ParentShopId.Value;
                    }
                }
            }

            var RegionNameList = new CommonLogic().RegionList();
            CreateShopMpdel model = new CreateShopMpdel()
            {
                RegionNameList = RegionNameList,
                StreetNameList = new CommonLogic().StreetList(Convert.ToInt32(RegionNameList[0].Value))
            };

            model.ParentShopId = parentShopId;
            return View(model);
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
        public ActionResult EditShop(
            EditShopMpdel model,
            HttpPostedFileBase[] ProductBrandFile,
            HttpPostedFileBase[] shopProductFile)
        {
            string oldMobile = "";

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "请检查输入的信息是否有问题");
                model.RegionNameList = new CommonLogic().RegionList(model.RegionId);
                model.StreetNameList = new CommonLogic().StreetList(model.RegionId, model.StreetId);
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ParentShopName)
                && !ShopLogic.CheckParentShopName(model.ParentShopName, model.ShopId))
            //&& !ShopLogic.CheckSelfShopId(model.ShopId))
            {
                ModelState.AddModelError("", "父店铺不合法");
                model.RegionNameList = new CommonLogic().RegionList(model.RegionId);
                model.StreetNameList = new CommonLogic().StreetList(model.RegionId, model.StreetId);
                return View(model);
            }

            try
            {
                using (var db = new MeiHiEntities())
                {
                    var shop = db.Shop.Where(a => a.ShopId == model.ShopId).FirstOrDefault();

                    if (shop == null)
                    {
                        ModelState.AddModelError("", "店铺不存在");
                        model.RegionNameList = new CommonLogic().RegionList(model.RegionId);
                        model.StreetNameList = new CommonLogic().StreetList(model.RegionId, model.StreetId);
                        return View(model);
                    }

                    oldMobile = shop.Phone;

                    if (oldMobile != model.Phone && ShopLogic.HaveRegisteredShopMobile(model.Phone))
                    {
                        ModelState.AddModelError("", "手机已经被注册");
                        model.RegionNameList = new CommonLogic().RegionList(model.RegionId);
                        model.StreetNameList = new CommonLogic().StreetList(model.RegionId, model.StreetId);
                        return View(model);
                    }

                    if (shop.Title != model.Title && ShopLogic.HaveRegisteredShopName(model.Title))
                    {
                        ModelState.AddModelError("", "店铺名已经被注册");
                        model.RegionNameList = new CommonLogic().RegionList(model.RegionId);
                        model.StreetNameList = new CommonLogic().StreetList(model.RegionId, model.StreetId);
                        return View(model);
                    }

                    //产品
                    if (ProductBrandFile != null && ProductBrandFile.FirstOrDefault() != null)
                    {
                        foreach (var item in shop.ProductBrand)
                        {
                            ImageHelper.DeleteImageFromDataBaseAndPhyclePath(item.ProductUrl, "/upload/shops/" + shop.Title + "/product/");
                        }

                        db.ProductBrand.RemoveRange(shop.ProductBrand);

                        var productBrandImages = ImageHelper.SaveImage(
                               Request.Url.Authority,
                               "/upload/shops/" + model.Title + "/product/",
                               ProductBrandFile);

                        foreach (var file in productBrandImages)
                        {
                            shop.ProductBrand.Add(new ProductBrand()
                            {
                                ProductUrl = file,
                                DateCreated = DateTime.Now
                            });
                        }
                    }

                    //店面
                    if (shopProductFile != null && shopProductFile.FirstOrDefault() != null)
                    {
                        foreach (var item in shop.ShopBrandImages)
                        {
                            ImageHelper.DeleteImageFromDataBaseAndPhyclePath(item.url, "/upload/shops/" + shop.Title + "/shop/");
                        }

                        db.ShopBrandImages.RemoveRange(shop.ShopBrandImages);

                        var shopBrandImages = ImageHelper.SaveImage(
                          Request.Url.Authority,
                          "/upload/shops/" + model.Title + "/shop/",
                          shopProductFile);

                        foreach (var file in shopBrandImages)
                        {
                            shop.ShopBrandImages.Add(new ShopBrandImages()
                            {
                                url = file,
                                DateCreated = DateTime.Now
                            });
                        }
                    }

                    shop.RegionID = model.RegionId;

                    if (model.StreetId == 0)
                    {
                        shop.StreetId = null;
                    }
                    else
                    {
                        shop.StreetId = model.StreetId;
                    }

                    shop.ShopTag = model.ShopTag;
                    shop.Comment = model.Comment;
                    shop.PurchaseNotes = model.PurchaseNotes;
                    shop.Phone = model.Phone;
                    shop.ParentShopId = ShopLogic.GetParentShopId(model.ParentShopName);
                    shop.IsOnline = model.IsOnline;
                    shop.IsHot = model.IsHot;
                    shop.Contract = model.Contract;
                    shop.Coordinates = model.Coordinates;
                    shop.Title = model.Title;
                    shop.DetailAddress = model.DetailAddress;
                    shop.DateModified = DateTime.Now;

                    var shopUser = shop.ShopUser.FirstOrDefault(a => a.ShopId == model.ShopId);

                    if (shopUser != null)
                    {
                        shopUser.WeiXinPayAccount = model.WinXinPayAccount;
                        shopUser.AliPayAccount = model.AliPayAccount;
                        shopUser.FullName = model.FullName;
                        shopUser.BankName = model.BankName;
                        shopUser.BankNo = model.BankNo;
                    }

                    db.SaveChanges();

                    if (oldMobile != model.Phone)
                    {
                        LuoSiMaoTextMessage.SendShopText(model.Phone, "");
                    }
                }

                return RedirectToAction("ShopManege");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                model.RegionNameList = new CommonLogic().RegionList();
                return View(model);
            }
        }

        private List<string> GetImages(IEnumerable<string> list)
        {
            List<string> result = new List<string>();

            if (list == null)
            {
                return result;
            }

            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    result.Add(item.Replace("100X100_", ""));
                }
            }

            return result;
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
                        ProductBrandList = shop.ProductBrand != null
                                            && shop.ProductBrand.Count > 0
                                            ? GetImages(shop.ProductBrand.Select(a => a.ProductUrl))
                                            : null,
                        ShopProductList = shop.ShopBrandImages != null
                                            && shop.ShopBrandImages.Count > 0
                                            ? GetImages(shop.ShopBrandImages.Select(a => a.url))
                                            : null
                    };

                    //model.RegionId = shop.RegionID;

                    if (shop.StreetId != null)
                    {
                        model.StreetNameList = new CommonLogic().StreetList(shop.RegionID, shop.StreetId.Value);
                        // model.StreetId = shop.StreetId.Value;

                    }
                    else
                    {
                        model.StreetNameList = new CommonLogic().StreetList(shop.RegionID);
                    }

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
                var shopUser = db.ShopUser.FirstOrDefault(a => a.ShopId == shopId);

                if (shop != null)
                {
                    CreateShopMpdel model = new CreateShopMpdel()
                    {
                        ShopId = shop.ShopId,
                        AliPayAccount = shopUser.AliPayAccount,
                        BankName = shopUser.BankName,
                        BankNo = shopUser.BankNo,
                        FullName = shopUser.FullName,
                        WinXinPayAccount = shopUser.WeiXinPayAccount,
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
                        ProductBrandList = shop.ProductBrand != null
                                            && shop.ProductBrand.Count > 0
                                            ? shop.ProductBrand.Select(a => a.ProductUrl).ToList()
                                            : null,
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

                    if (shop.Service != null && shop.Service.Count() > 0)
                    {
                        foreach (var service in shop.Service)
                        {
                            access.UserFavorites.RemoveRange(service.UserFavorites);
                        }

                        access.Service.RemoveRange(shop.Service);
                    }

                    access.ShopUser.RemoveRange(shop.ShopUser);

                    if (shop.ProductBrand != null && shop.ProductBrand.Count > 0)
                    {

                        foreach (var item in shop.ProductBrand)
                        {
                            ImageHelper.DeleteImageFromDataBaseAndPhyclePath(item.ProductUrl, "/upload/shops/" + shop.Title + "/product/");
                        }
                        access.ProductBrand.RemoveRange(shop.ProductBrand);

                    }

                    if (shop.ShopBrandImages != null && shop.ShopBrandImages.Count > 0)
                    {

                        foreach (var item in shop.ShopBrandImages)
                        {
                            ImageHelper.DeleteImageFromDataBaseAndPhyclePath(item.url, "/upload/shops/" + shop.Title + "/shop/");
                        }
                        access.ShopBrandImages.RemoveRange(shop.ShopBrandImages);

                    }


                    if (shop.UserComments != null && shop.UserComments.Count > 0)
                    {
                        foreach (var userComment in shop.UserComments)
                        {
                            access.UserCommentSharedImg.RemoveRange(userComment.UserCommentSharedImg);
                            access.UserCommentsReply.RemoveRange(userComment.UserCommentsReply);
                        }

                        access.UserComments.RemoveRange(shop.UserComments);
                    }

                    access.UserFavorites.RemoveRange(shop.UserFavorites);
                    access.Shop.Remove(shop);
                    access.SaveChanges();

                    var childShops = access.Shop.Where(a => a.ParentShopId == shop.ParentShopId && a.ShopId != shop.ShopId);

                    foreach (var item in childShops)
                    {
                        DeleteShop(item.ShopId);
                    }
                }
            }

            return RedirectToAction("ShopManege");
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult DropDownShop(long shopId)
        {
            using (var access = new MeiHiEntities())
            {
                var shop = access.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                if (shop != null)
                {
                    shop.IsOnline = false;
                    access.SaveChanges();
                }
            }

            return RedirectToAction("ShopManege");
        }

        [HttpGet]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult SetUpShop(long shopId)
        {
            using (var access = new MeiHiEntities())
            {
                var shop = access.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                if (shop != null)
                {
                    shop.IsOnline = true;
                    access.SaveChanges();
                }
            }

            return RedirectToAction("OfflineShopManage");
        }
        #endregion

        #region service
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult SaveService(
            CreateServiceModel model,
            HttpPostedFileBase[] serviceTitleUrl)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    var service = db.Service.Where(a => a.ServiceId == model.ServiceId).FirstOrDefault();


                    if (service == null)
                    {
                        service = new Service();

                        if (serviceTitleUrl != null && serviceTitleUrl.Count() > 0 && serviceTitleUrl[0] != null)
                        {
                            var serviceImageUrl = ImageHelper.SaveImage(
                                Request.Url.Authority,
                                "/upload/shops/" + ShopLogic.GetShopNameByShopId(model.ShopId) + "/service/",
                                serviceTitleUrl);
                            service.TitleUrl = serviceImageUrl[0];
                        }

                        service.ServiceTypeId = model.ServiceTypeId;
                        service.CMUnitCost = model.CMUnitCost;
                        service.Designer = model.Designer;
                        service.Detail = model.Detail;
                        service.OriginalUnitCost = model.OriginalUnitCost;
                        service.Title = model.Title;
                        service.ShopId = model.ShopId;
                        service.IfSupportRealTimeRefund = true;
                        service.DateCreated = DateTime.Now;
                        service.DateModified = DateTime.Now;
                        db.Service.Add(service);
                    }
                    else
                    {
                        if (serviceTitleUrl != null && serviceTitleUrl.Count() > 0 && serviceTitleUrl[0] != null)
                        {
                            var serviceImageUrl = ImageHelper.SaveImage(
                                Request.Url.Authority,
                                "/upload/shops/" + service.Shop.Title + "/service/",
                                serviceTitleUrl);

                            if (!string.IsNullOrEmpty(service.TitleUrl))
                            {
                                ImageHelper.DeleteImageFromDataBaseAndPhyclePath(
                                service.TitleUrl,
                                "/upload/shops/" + service.Shop.Title + "/service/");
                            }

                            service.TitleUrl = serviceImageUrl[0];
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
                model.ShopName = access.Shop.First(a => a.ShopId == shopId).Title;
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
                    if (!string.IsNullOrEmpty(service.TitleUrl))
                    {
                        ImageHelper.DeleteImageFromDataBaseAndPhyclePath(
                                 service.TitleUrl,
                                 "/upload/shops/" + service.Shop.Title + "/service/");
                    }

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

        #region servicetype
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult ManageServiceType()
        {
            using (var db = new MeiHiEntities())
            {
                List<ServiceTypeModel> model = new List<ServiceTypeModel>();

                db.ServiceType.ToList().ForEach(a => model.Add(new ServiceTypeModel()
                {
                    ServiceTypeId = a.ServiceTypeId,
                    ServiceTypeName = a.Title
                }));

                return View(model);
            }
        }
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult CreateServiceType()
        {
            return View(new ServiceTypeModel());
        }
        [Auth(PermissionName = "店铺维护管理")]
        [HttpPost]
        public ActionResult CreateServiceType(ServiceTypeModel model)
        {
            using (var db = new MeiHiEntities())
            {
                db.ServiceType.Add(new ServiceType() { DateCreated = DateTime.Now, Title = model.ServiceTypeName });
                db.SaveChanges();

                return RedirectToAction("ManageServiceType");
            }
        }
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult EditServiceType(int serviceTypeId)
        {
            using (var db = new MeiHiEntities())
            {
                var srrvicetype = db.ServiceType.FirstOrDefault(a => a.ServiceTypeId == serviceTypeId);
                var model = new ServiceTypeModel();
                model.ServiceTypeId = serviceTypeId;
                model.ServiceTypeName = srrvicetype.Title;

                return View(model);
            }
        }
        [Auth(PermissionName = "店铺维护管理")]
        [HttpPost]
        public ActionResult EditServiceType(ServiceTypeModel model)
        {
            using (var db = new MeiHiEntities())
            {
                var serviceType = db.ServiceType.FirstOrDefault(a => a.ServiceTypeId == model.ServiceTypeId);

                serviceType.Title = model.ServiceTypeName;
                db.SaveChanges();

                return RedirectToAction("ManageServiceType");
            }
        }
        [Auth(PermissionName = "店铺维护管理")]
        public ActionResult DeleteServiceType(int serviceTypeId)
        {
            using (var db = new MeiHiEntities())
            {
                db.ServiceType.Remove(db.ServiceType.First(a => a.ServiceTypeId == serviceTypeId));
                db.SaveChanges();

                return RedirectToAction("ManageServiceType");
            }
        }
        #endregion
    }
}