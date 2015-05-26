using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;
using MeiHi.Shop.ViewModels;

namespace MeiHi.Shop.Controllers
{
    public class HomeController : Controller
    {
        [Auth(Roles = "店主")]
        public ActionResult Index()
        {
            ViewBag.ShopId = Session["ShopId"];

            return View();
        }

        [Auth(Roles = "店主")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult VerifyUserMeiHiCode(string code)
        {
            using (var db = new MeiHiEntities())
            {
                var booking = db.Booking.FirstOrDefault(a => a.VerifyCode == code && !a.IsUsed && a.IsBilling);

                if (booking != null)
                {
                    booking.IsUsed = true;
                    db.SaveChanges();
                    ViewBag.WelcomeMessage = string.Format("用户[{0}] 购买的 [{1} {2} {3}] 验证通过, 下单时间 {4},下单手机号 {5}",
                                                    booking.User.FullName,
                                                    booking.ShopName,
                                                    booking.ServiceName,
                                                    booking.Designer,
                                                    booking.DateModified,
                                                    booking.Mobile);
                }
                else
                {
                    ViewBag.ErrorMessage = string.Format("美嗨券 {0} 验证失败, 请核对后重新输入", code);
                }

                return RedirectToAction("Index");
            }
        }

        [Auth(Roles = "店主")]
        [HttpGet]
        public ActionResult ResetShoperPassword(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shoper = db.ShopUser.FirstOrDefault(a => a.ShopId == shopId);

                if (shoper == null)
                {
                    throw new Exception("无效的店铺");
                }

                var shoperModel = new ShoperModel();
                shoperModel.ShopId = shopId;
                //shoperModel.Password = shoper.Password;

                return View(shoperModel);
            }
        }

        [Auth(Roles = "店主")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateShopper(ShoperModel model)
        {
            using (var db = new MeiHiEntities())
            {
                if (model.Password1 != model.Password2)
                {
                    TempData["message"] = "两个密码不一致,请重新输入";
                }
                else
                {
                    var shoper = db.ShopUser.FirstOrDefault(a => a.ShopId == model.ShopId);

                    if (shoper == null)
                    {
                        throw new Exception("无效的店铺");
                    }

                    shoper.Password = model.Password1;
                    db.SaveChanges();

                    TempData["message"] = "重置成功";
                }

                return RedirectToAction("ResetShoperPassword", new { shopId = model.ShopId });
            }
        }
    }
}