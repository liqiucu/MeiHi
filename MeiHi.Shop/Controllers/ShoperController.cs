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
using System.Data.Entity.Validation;
using System.IO;
using MeiHi.CommonDll.Helper;
using MeiHi.Shop.Logic;
using MeiHi.Shop.Models.Shoper;
using MeiHi.Shop.Models.User;
using MeiHi.Shop.Models.Booking;
using MeiHi.Shop.Models.HomeModel;

namespace MeiHi.Shop.Controllers
{
    public class ShoperController : Controller
    {
        // GET: Shoper

        [Auth(Roles = "店主")]
        public ActionResult VerifyUserMeiHiCode(long shopId)
        {
            try
            {
                using (var db = new MeiHiEntities())
                {
                    ShoperHomeModel model = new ShoperHomeModel();

                    var shop = db.Shop.FirstOrDefault(a => a.ShopId == shopId);

                    if (shop != null)
                    {
                        model.ShopId = shop.ShopId;
                        model.ShopName = shop.Title;
                    }
                    else
                    {
                        ModelState.AddModelError("", "店铺信息不存在 请重新登陆 如还未解决问题 请联系美嗨客服！");
                    }

                    return View(model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Auth(Roles = "店主")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult VerifyUserMeiHiCode(ShoperHomeModel model)
        {
            using (var db = new MeiHiEntities())
            {
                var booking = db.Booking.FirstOrDefault(a => a.VerifyCode == model.MeiHiCode && !a.IsUsed && a.IsBilling);

                if (booking != null)
                {
                    booking.IsUsed = true;
                    db.SaveChanges();
                    TempData["WelcomeMessage"] = string.Format("用户[{0}] 购买的 [{1} {2} {3}] 验证通过, 下单时间 {4},下单手机号 {5},订单号 {6}",
                                                    booking.User.FullName,
                                                    booking.ShopName,
                                                    booking.ServiceName,
                                                    booking.Designer,
                                                    booking.DateModified,
                                                    booking.Mobile,
                                                    booking.BookingId);
                }
                else
                {
                    ModelState.AddModelError("", string.Format("美嗨券 {0} 验证失败, 请核对后重新输入", model.MeiHiCode));
                }

                return View(model);
            }
        }
    }
}