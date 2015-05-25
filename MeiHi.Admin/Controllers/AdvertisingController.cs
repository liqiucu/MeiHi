using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;
using MeiHi.Admin.Models.Advertising;
using MeiHi.CommonDll.Helper;

namespace MeiHi.Admin.Controllers
{
    public class AdvertisingController : Controller
    {
        // GET: Advertising
        [Auth(PermissionName = "广告位管理")]
        public ActionResult ShowAdvertising()
        {
            using (var db = new MeiHiEntities())
            {
                var adds = db.Add;

                List<AdvertisingModel> result = new List<AdvertisingModel>();

                foreach (var add in adds)
                {
                    result.Add(new AdvertisingModel()
                    {
                        Url = add.Url,
                        Avaliable = add.Avaliable != null ? add.Avaliable.Value : false,
                        Title = add.Title,
                        AddId = add.AddId
                    });
                }

                return View(result);
            }
        }

        [HttpGet]
        [Auth(PermissionName = "广告位管理")]
        public ActionResult CreateAdd()
        {
            var model = new AdvertisingModel();
            return View(model);
        }

        [HttpPost]
        [Auth(PermissionName = "广告位管理")]
        public ActionResult SaveAdd(AdvertisingModel model, HttpPostedFileBase[] addImages)
        {
            using (var db = new MeiHiEntities())
            {
                if (addImages != null && addImages.Count() == 1)
                {

                    var add = new Add()
                    {
                        Avaliable = model.Avaliable,
                        DateCreated = DateTime.Now,
                        Title = model.Title
                    };

                    var serviceImageUrl = ImageHelper.SaveImage(
                        Request.Url.Authority,
                        "/upload/home/add/",
                        addImages, 0, 0, 100);

                    add.Url = serviceImageUrl[0];

                    db.Add.Add(add);
                    db.SaveChanges();

                    return RedirectToAction("ShowAdvertising");
                }
                else
                {
                    return RedirectToAction("ShowAdvertising");
                }
            }
        }

        [HttpGet]
        [Auth(PermissionName = "广告位管理")]
        public ActionResult EditAdd(long addId)
        {
            using (var db = new MeiHiEntities())
            {
                var add = db.Add.FirstOrDefault(a => a.AddId == addId);

                if (add == null)
                {
                    throw new Exception("没有获取到广告位 addid:" + addId);
                }

                var model = new AdvertisingModel()
                {
                    Url = add.Url,
                    Avaliable = add.Avaliable != null ? add.Avaliable.Value : false,
                    Title = add.Title,
                    AddId = add.AddId
                };

                return View(model);
            }
        }

        [HttpGet]
        [Auth(PermissionName = "广告位管理")]
        public ActionResult UpdateAdd(AdvertisingModel model)
        {
            using (var db = new MeiHiEntities())
            {
                var add = db.Add.FirstOrDefault(a => a.AddId == model.AddId);
                if (add == null)
                {
                    throw new Exception("没有获取到广告位 addid:" + model.AddId);
                }

                add.Avaliable = model.Avaliable;
                add.DateCreated = DateTime.Now;
                add.Title = model.Title;
                add.Url = model.Url;

                db.SaveChanges();

                return RedirectToAction("ShowAdvertising");
            }
        }

        [HttpGet]
        [Auth(PermissionName = "广告位管理")]
        public ActionResult DeleteAdd(long addId)
        {
            using (var db = new MeiHiEntities())
            {
                var add = db.Add.FirstOrDefault(a => a.AddId == addId);
                if (add == null)
                {
                    throw new Exception("没有获取到广告位 addid:" + addId);
                }

                db.Add.Remove(add);
                db.SaveChanges();
                return RedirectToAction("ShowAdvertising");
            }
        }
    }
}