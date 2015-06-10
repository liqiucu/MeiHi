using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;
using MeiHi.Admin.Models.HairStyle;
using MeiHi.CommonDll.Helper;

namespace MeiHi.Admin.Controllers
{
    public class HairStyleController : Controller
    {
        #region type

        [HttpGet]
        public ActionResult ManageHairStyleType()
        {
            using (var db = new MeiHiEntities())
            {
                var models = new List<HairStyleTypeModel>();
                var stypes = db.HairStyleType;

                foreach (var item in stypes)
                {
                    if (item != null)
                    {
                        models.Add(new HairStyleTypeModel()
                        {
                            HairStyleTypeId = item.HairStyleTypeId,
                            HairStyleTypeName = item.Title
                        });
                    }
                }

                return View(models);
            }
        }

        [HttpGet]
        public ActionResult CreateHairStyleType()
        {
            var models = new HairStyleTypeModel();
            return View(models);
        }

        [HttpGet]
        public ActionResult EditHairStyleType(long hairStyleTypeId)
        {
            using (var db = new MeiHiEntities())
            {
                var styleType = db.HairStyleType.FirstOrDefault(a => a.HairStyleTypeId == hairStyleTypeId);

                if (styleType == null)
                {
                    throw new Exception("没有对应的发型类型");
                }

                HairStyleTypeModel model = new HairStyleTypeModel()
                {
                    HairStyleTypeId = styleType.HairStyleTypeId,
                    HairStyleTypeName = styleType.Title
                };

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult EditHairStyleType(HairStyleTypeModel model)
        {
            using (var db = new MeiHiEntities())
            {
                var styleType = db.HairStyleType.FirstOrDefault(a => a.HairStyleTypeId == model.HairStyleTypeId);

                if (styleType == null)
                {
                    ModelState.AddModelError("", "没有对应的发型类型");
                    return View(model);
                }

                styleType.DateCreated = DateTime.Now;
                styleType.Title = model.HairStyleTypeName;
                db.SaveChanges();

                return RedirectToAction("ManageHairStyleType");
            }
        }

        [HttpPost]
        public ActionResult SaveHairStyleType(HairStyleTypeModel model)
        {
            using (var db = new MeiHiEntities())
            {
                db.HairStyleType.Add(new HairStyleType()
                {
                    DateCreated = DateTime.Now,
                    Title = model.HairStyleTypeName
                });

                db.SaveChanges();

                return RedirectToAction("ManageHairStyleType");
            }
        }

        [HttpGet]
        public ActionResult DeleteHairStyleType(long hairStyleTypeId)
        {
            using (var db = new MeiHiEntities())
            {
                var styleType = db.HairStyleType.FirstOrDefault(a => a.HairStyleTypeId == hairStyleTypeId);

                if (styleType == null)
                {
                    throw new Exception("没有对应的发型类型");
                }

                ImageHelper.DeleteAllImagesFromPhycleFolderPath(
                                    "/upload/HairStyleTypes/" + hairStyleTypeId + "/");

                db.HairStyle.RemoveRange(styleType.HairStyle);
                db.HairStyleType.Remove(styleType);
                db.SaveChanges();

                return RedirectToAction("ManageHairStyleType");
            }
        }
        #endregion

        [HttpGet]
        public ActionResult ManageHairStyle(long hairStyleTypeId)
        {
            using (var db = new MeiHiEntities())
            {
                var styleType = db.HairStyleType.FirstOrDefault(a => a.HairStyleTypeId == hairStyleTypeId);


                if (styleType == null)
                {
                    throw new Exception("没有对应的发型类型");
                }

                var result = new HairStylesModel();
                result.HairStyleTypeId = styleType.HairStyleTypeId;
                result.HairStyleTypeName = styleType.Title;

                var temp = new List<HairStyleModel>();

                foreach (var item in styleType.HairStyle)
                {
                    if (item != null)
                    {
                        temp.Add(new HairStyleModel()
                        {
                            HairStyleId = item.HairStyleId,
                            ModelUrl = item.HairStyleModelUrl,
                            StyleUrl = item.HairStyleUrl
                        });
                    }
                }

                result.Styles = temp;
                return View(result);
            }
        }

        [HttpGet]
        public ActionResult CreateHairStyle(long hairStyleTypeId)
        {
            using (var db = new MeiHiEntities())
            {
                var type = db.HairStyleType.FirstOrDefault(a => a.HairStyleTypeId == hairStyleTypeId);

                var model = new HairStyleModel()
                {
                    HairStyleTypeId = hairStyleTypeId,
                    HairStyleTypeName = type.Title
                };

                return View(model);
            }

        }

        [HttpPost]
        public ActionResult SaveHairStyle(
            HairStyleModel model,
            HttpPostedFileBase[] modelUrls,
            HttpPostedFileBase[] styleUrls)
        {
            if (modelUrls.Count() != 1 || styleUrls.Count() != 1)
            {
                throw new Exception("图片上传数量必须是1");
            }

            var modelUrl = ImageHelper.SaveImage(
                                Request.Url.Authority,
                                "/upload/HairStyleTypes/" + model.HairStyleTypeId + "/Model/",
                               modelUrls);

            var stypeUrl = ImageHelper.SaveImage(
                               Request.Url.Authority,
                               "/upload/HairStyleTypes/" + model.HairStyleTypeId + "/Style/",
                              styleUrls);

            using (var db = new MeiHiEntities())
            {
                db.HairStyle.Add(new HairStyle()
                {
                    DateCreated = DateTime.Now,
                    HairStyleTypeId = model.HairStyleTypeId,
                    HairStyleUrl = stypeUrl[0],
                    HairStyleModelUrl = modelUrl[0]
                });

                db.SaveChanges();
            }

            return RedirectToAction("ManageHairStyle", new { hairStyleTypeId = model.HairStyleTypeId });
        }

        [HttpGet]
        public ActionResult EditHairStyle(long hairStyleId)
        {
            using (var db = new MeiHiEntities())
            {
                var style = db.HairStyle.FirstOrDefault(a => a.HairStyleId == hairStyleId);

                if (style == null)
                {
                    throw new Exception("发型不存在");
                }

                HairStyleModel result = new HairStyleModel()
                {
                    HairStyleId = style.HairStyleId,
                    HairStyleTypeName = style.HairStyleType.Title,
                    HairStyleTypeId = style.HairStyleTypeId,
                    ModelUrl = style.HairStyleModelUrl,
                    StyleUrl = style.HairStyleUrl
                };

                return View(result);
            }
        }

        [HttpPost]
        public ActionResult UpdateHairStyle(
            HairStyleModel model,
            HttpPostedFileBase[] modelUrls,
            HttpPostedFileBase[] styleUrls)
        {
            if (
                (modelUrls == null && styleUrls == null)
                ||
                (modelUrls.Count() == 0 && styleUrls.Count() == 0)
                )
            {
                throw new Exception("图片必须上传");
            }

            using (var db = new MeiHiEntities())
            {
                var style = db.HairStyle.FirstOrDefault(a => a.HairStyleId == model.HairStyleId);

                if (modelUrls != null && modelUrls.Count() == 1)
                {
                    ImageHelper.DeleteImageFromDataBaseAndPhyclePath(
                                     style.HairStyleModelUrl,
                                      "/upload/HairStyleTypes/" + style.HairStyleTypeId + "/Model/");

                    var modelUrl = ImageHelper.SaveImage(
                                Request.Url.Authority,
                                "/upload/HairStyleTypes/" + style.HairStyleTypeId + "/Model/",
                               modelUrls);

                    style.DateCreated = DateTime.Now;
                    style.HairStyleModelUrl = modelUrl[0];
                }

                if (styleUrls != null && styleUrls.Count() == 1)
                {
                    ImageHelper.DeleteImageFromDataBaseAndPhyclePath(
                                     style.HairStyleModelUrl,
                                      "/upload/HairStyleTypes/" + style.HairStyleTypeId + "/Style/");

                    var styleUrl = ImageHelper.SaveImage(
                                Request.Url.Authority,
                                "/upload/HairStyleTypes/" + style.HairStyleTypeId + "/Style/",
                               styleUrls);

                    style.DateCreated = DateTime.Now;
                    style.HairStyleUrl = styleUrl[0];
                }

                db.SaveChanges();
            }

            return RedirectToAction("ManageHairStyle", new { hairStyleTypeId = model.HairStyleTypeId });
        }

        [HttpGet]
        public ActionResult DeleteHairStyle(long hairStyleId)
        {
            using (var db = new MeiHiEntities())
            {
                var style = db.HairStyle.FirstOrDefault(a => a.HairStyleId == hairStyleId);

                if (style != null)
                {
                    ImageHelper.DeleteImageFromDataBaseAndPhyclePath(
                                    style.HairStyleModelUrl,
                                     "/upload/HairStyleTypes/" + style.HairStyleTypeId + "/Model/");

                    ImageHelper.DeleteImageFromDataBaseAndPhyclePath(
                                   style.HairStyleUrl,
                                    "/upload/HairStyleTypes/" + style.HairStyleTypeId + "/Style/");
                    db.HairStyle.Remove(style);
                    db.SaveChanges();
                }

                return RedirectToAction("ManageHairStyle", new { hairStyleTypeId = style.HairStyleTypeId });
            }
        }
    }
}