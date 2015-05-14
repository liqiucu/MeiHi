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
        public ActionResult ShopManege(int page = 1)
        {
            ShopModel shopmodel = new ShopModel();
            shopmodel.Lists = ShopLogic.GetShops(page, 10);
            return View(shopmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveShop(CreateShopMpdel model, HttpPostedFileBase[] fileToUpload, HttpPostedFileBase[] fileToUploadForShop)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    string productBrandId = Guid.NewGuid().ToString();


                    foreach (HttpPostedFileBase file in fileToUpload)
                    {
                        string path = System.IO.Path.Combine(Server.MapPath("~/App_Data"), System.IO.Path.GetFileName(file.FileName));
                        file.SaveAs(path);

                        db.ProductBrand.Add(new ProductBrand()
                        {
                            ProductUrl = path,
                            ProductBrandId = productBrandId,
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now
                        });
                    }

                    db.SaveChanges();

                    string tempPath = "";
                    if (fileToUploadForShop != null && fileToUploadForShop.Count() > 0)
                    {
                        tempPath = System.IO.Path.Combine(Server.MapPath("~/App_Data"), System.IO.Path.GetFileName(fileToUploadForShop[0].FileName));
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

                    foreach (var file in fileToUploadForShop)
                    {
                        string path = System.IO.Path.Combine(Server.MapPath("~/App_Data"), System.IO.Path.GetFileName(file.FileName));

                        file.SaveAs(path);

                        db.ShopBrandImages.Add(new ShopBrandImages()
                        {
                            ShopId = ShopLogic.GetShopIdByShopName(model.Title),
                            DateCreated = DateTime.Now,
                            url = path
                        });
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

        [HttpGet]
        public ActionResult CreateShop()
        {
            CreateShopMpdel model = new CreateShopMpdel()
            {
                RegionNameList = new CommonLogic().RegionList()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult EditShop(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                if (shop != null)
                {
                    EditShopMpdel model = new EditShopMpdel()
                    {
                        RegionNameList = new CommonLogic().RegionList(),
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
        public ActionResult ShopDetail(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shop = db.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                if (shop != null)
                {
                    ShopDetailMpdel model = new ShopDetailMpdel()
                    {
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
        public ActionResult DeleteShop(long shopId)
        {
            using (var access = new MeiHiEntities())
            {
                var shop = access.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                if (shop != null)
                {
                    access.Shop.Attach(shop);
                    access.Shop.Remove(shop);
                    access.SaveChanges();
                }
            }

            return RedirectToAction("ShopManege");
        }
    }
}