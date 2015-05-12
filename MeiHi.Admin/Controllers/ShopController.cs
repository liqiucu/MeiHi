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
            shopmodel.Lists = new ShopLogic().GetShops(page, 10);
            return View(shopmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveShop(CreateShopMpdel model, HttpPostedFileBase[] fileToUpload)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    string productBrandId = Guid.NewGuid().ToString();
                    string imageTitleUrl = "";

                    foreach (HttpPostedFileBase file in fileToUpload)
                    {
                        string path = System.IO.Path.Combine(Server.MapPath("~/App_Data"), System.IO.Path.GetFileName(file.FileName));
                        file.SaveAs(path);

                        if (string.IsNullOrEmpty(imageTitleUrl))
                        {
                            imageTitleUrl = path;
                        }

                        db.ProductBrand.Add(new ProductBrand()
                        {
                            ProductUrl = path,
                            ProductBrandId = productBrandId,
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now
                        });
                    }

                    db.SaveChanges();

                    db.Shop.Add(new Shop()
                    {
                        RegionID = model.RegionId,
                        ShopTag = model.ShopTag,
                        Comment = model.Comment,
                        PurchaseNotes = model.PurchaseNotes,
                        ProductBrandId = productBrandId,
                        Phone = model.Phone,
                        ParentShopId = new ShopLogic().GetParentShopId(model.ParentShopName),
                        IsOnline = model.IsOnline,
                        IsHot = model.IsHot,
                        Contract = model.Contract,
                        Coordinates = model.Coordinates,
                        Title = model.Title,
                        ImageUrl = imageTitleUrl,
                        DetailAddress = model.DetailAddress,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now
                    });

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
            CreateShopMpdel model = new CreateShopMpdel()
            {
                RegionNameList = new CommonLogic().RegionList()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult ShopDetail(long shopId)
        {
            CreateShopMpdel model = new CreateShopMpdel()
            {
                RegionNameList = new CommonLogic().RegionList()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteShop(long shopId)
        {
            using (var access = new MeiHiEntities())
            {
                var shop = access.Shop.Where(a => a.ShopId == shopId).FirstOrDefault();

                if (shop != null)
                {
                    access.Shop.Remove(shop);

                    access.SaveChanges();
                }
            }

            return RedirectToAction("ShopManege");
        }
    }
}