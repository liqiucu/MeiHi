using MeiHi.Admin.Logic;
using MeiHi.Admin.Models.City;
using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeiHi.Admin.Controllers
{
    public class CityController : Controller
    {
        // GET: CityRegion
        public ActionResult ManageCityRegions(int cityId = 20)
        {
            var model = CityLogic.GetAllRegionsByCityId(cityId);

            return View(model);
        }

        public ActionResult CreateRegionByCityId(int cityId = 20)
        {
            using (var db = new MeiHiEntities())
            {
                RegionModel model = new RegionModel();

                var city = db.Region.FirstOrDefault(a => a.RegionId == cityId);

                if (city == null)
                {
                    ModelState.AddModelError("", "城市不存在");
                    return View(model);
                }

                model.ParentRegionName = city.Name;
                model.ParentRegionId = cityId;
                model.ProvinceId = city.ProvinceId.Value;
                model.RootRegionId = cityId;

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult CreateRegionByCityId(RegionModel model)
        {
            using (var db = new MeiHiEntities())
            {
                db.Region.Add(new Region()
                {
                    RootRegionId = model.RootRegionId,
                    Name = model.RegionName,
                    ParentRegionId = model.ParentRegionId,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    ProvinceId = model.ProvinceId
                });

                db.SaveChanges();

                return RedirectToAction("ManageCityRegions", new { cityId = model.ParentRegionId });
            }
        }

        public ActionResult EditRegion(int regionId)
        {
            using (var db = new MeiHiEntities())
            {
                RegionModel model = new RegionModel();

                var region = db.Region.FirstOrDefault(a => a.RegionId == regionId);

                if (region == null)
                {
                    ModelState.AddModelError("", "区域不存在");
                    return View(model);
                }

                model.ParentRegionId = region.ParentRegionId.Value;
                model.RegionId = regionId;
                model.RegionName = region.Name;

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult EditRegion(RegionModel model)
        {
            using (var db = new MeiHiEntities())
            {
                var region = db.Region.FirstOrDefault(a => a.RegionId == model.RegionId);

                if (region == null)
                {
                    ModelState.AddModelError("", "区域不存在");
                    return View(model);
                }

                region.Name = model.RegionName;
                db.SaveChanges();
                return RedirectToAction("ManageCityRegions", new { cityId = region.ParentRegionId });
            }
        }

        public ActionResult DeleteRegion(int regionId)
        {
            using (var db = new MeiHiEntities())
            {
                var region = db.Region.FirstOrDefault(a => a.RegionId == regionId);

                if (region != null)
                {
                    var streets = db.Region.Where(a => a.ParentRegionId == regionId);
                    db.Region.Remove(region);
                    db.Region.RemoveRange(streets);
                    db.SaveChanges();
                }

                return RedirectToAction("ManageCityRegions", new { cityId = region.ParentRegionId });
            }
        }

        public ActionResult DeleteStreet(int regionId)
        {
            using (var db = new MeiHiEntities())
            {
                var region = db.Region.FirstOrDefault(a => a.RegionId == regionId);

                if (region != null)
                {
                    db.Region.Remove(region);
                    db.SaveChanges();
                }

                return RedirectToAction("StreetManage", new { regionId = region.ParentRegionId });
            }
        }

        public ActionResult EditStreet(int regionId)
        {
            using (var db = new MeiHiEntities())
            {
                RegionModel model = new RegionModel();

                var region = db.Region.FirstOrDefault(a => a.RegionId == regionId);

                if (region == null)
                {
                    ModelState.AddModelError("", "街道不存在");
                    return View(model);
                }

                model.ParentRegionId = region.ParentRegionId.Value;
                model.RegionId = regionId;
                model.RegionName = region.Name;

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult EditStreet(RegionModel model)
        {
            using (var db = new MeiHiEntities())
            {
                var region = db.Region.FirstOrDefault(a => a.RegionId == model.RegionId);

                if (region == null)
                {
                    ModelState.AddModelError("", "街道不存在");
                    return View(model);
                }

                region.Name = model.RegionName;
                db.SaveChanges();
                return RedirectToAction("StreetManage", new { regionId = region.ParentRegionId });
            }
        }

        public ActionResult CreateStreetByRegionId(int regionId)
        {
            using (var db = new MeiHiEntities())
            {
                RegionModel model = new RegionModel();

                var region = db.Region.FirstOrDefault(a => a.RegionId == regionId);

                if (region == null)
                {
                    ModelState.AddModelError("", "区域不存在");
                    return View(model);
                }

                model.ParentRegionName = region.Name;
                model.ParentRegionId = regionId;
                model.ProvinceId = region.ProvinceId.Value;
                model.RootRegionId = region.RootRegionId.Value;

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult CreateStreetByRegionId(RegionModel model)
        {
            using (var db = new MeiHiEntities())
            {
                db.Region.Add(new Region()
                {
                    RootRegionId = model.RootRegionId,
                    Name = model.RegionName,
                    ParentRegionId = model.ParentRegionId,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    ProvinceId = model.ProvinceId
                });

                db.SaveChanges();

                return RedirectToAction("StreetManage", new { regionId = model.ParentRegionId });
            }
        }

        public ActionResult StreetManage(int regionId)
        {
            var model = CityLogic.GetAllStreetsByRegionId(regionId);

            return View(model);
        }
    }
}