using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;
using MeiHi.Shop.ViewModels;
using MeiHi.Shop.Models.HomeModel;

namespace MeiHi.Shop.Controllers
{
    public class HomeController : Controller
    {
        [Auth(Roles = "店主")]
        public ActionResult Index()
        {
            using (var db = new MeiHiEntities())
            {
                ShoperHomeModel model = new ShoperHomeModel();

                if (Session["ShopId"] != null)
                {
                    var shopId = long.Parse(Session["ShopId"].ToString());
                    TempData["ShopId"] = Session["ShopId"];

                    var shop = db.Shop.FirstOrDefault(a => a.ShopId == shopId);

                    if (shop != null)
                    {
                        model.ShopId = shop.ShopId;
                        model.ShopName = shop.Title;
                    }
                }

                return View(model);
            }
        }

        [Auth(Roles = "店主")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult VerifyUserMeiHiCode(string code, long shopId, string shopName)
        {
            using (var db = new MeiHiEntities())
            {
                var booking = db.Booking.FirstOrDefault(a => a.VerifyCode == code && !a.IsUsed && a.IsBilling);

                if (booking != null)
                {
                    booking.IsUsed = true;
                    db.SaveChanges();
                    TempData["WelcomeMessage"] = string.Format("用户[{0}] 购买的 [{1} {2} {3}] 验证通过, 下单时间 {4},下单手机号 {5}",
                                                    booking.User.FullName,
                                                    booking.ShopName,
                                                    booking.ServiceName,
                                                    booking.Designer,
                                                    booking.DateModified,
                                                    booking.Mobile);
                }
                else
                {
                    TempData["ErrorMessage"] = string.Format("美嗨券 {0} 验证失败, 请核对后重新输入", code);
                }

                return RedirectToAction("Index", new ShoperHomeModel() { ShopId = shopId, ShopName = shopName });
            }
        }

        [Auth(Roles = "店主")]
        public ActionResult ResetShoperPassword(long shopId)
        {
            var shoperModel = new ShoperModelPassword();
            shoperModel.ShopId = shopId;
            return View(shoperModel);
        }

        [Auth(Roles = "店主")]
        [HttpPost]
        public ActionResult ResetShoperPassword(ShoperModelPassword model)
        {
            using (var db = new MeiHiEntities())
            {
                if (model.Password1 != model.Password2)
                {
                    ModelState.AddModelError("", "两个密码不一致,请重新输入");
                    return View(model);
                }
                else
                {
                    var shoper = db.ShopUser.FirstOrDefault(a => a.ShopId == model.ShopId);

                    if (shoper == null)
                    {
                        ModelState.AddModelError("", "无效的店铺");
                        return View(model);
                    }

                    shoper.Password = model.Password1;
                    db.SaveChanges();

                    TempData["message"] = "重置成功";

                    return View(model);
                }
            }
        }
    }
}