
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;
using MeiHi.Shop.ViewModels;
using System.Web.Security;
using MeiHi.Shop.Logic;

namespace MeiHi.Shop.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Session["permissionnotenough"] != null)
            {
                Session["permissionnotenough"] = null;
                ViewBag.permissionnotenough = true;
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            long shopId = 0;

            using (var db = new MeiHiEntities())
            {
                if (ModelState.IsValid && ShoperLogic.Logon(model.UserName, model.Password, out shopId))
                {
                    Session["ShopId"] = shopId;
                    Session["UserName"] = db.ShopUser.FirstOrDefault(a => a.ShopId == shopId).Shop.Title;

                    if (Session["ReturnUrl"] != null)
                    {
                        return Redirect(Session["ReturnUrl"].ToString());
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}