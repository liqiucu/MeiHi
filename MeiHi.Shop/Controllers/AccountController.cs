
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;
using MeiHi.Shop.ViewModels;
using System.Web.Security;
using MeiHi.Shop.Logic;
using MeiHi.CommonDll;

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
            if (Session["ValidateCode"] != null && Session["ValidateCode"].ToString() != model.ValidateCode)
            {
                ModelState.AddModelError("", "验证码错误，请从新输入");

                return View(model);
            }

            long shopId = 0;

            using (var db = new MeiHiEntities())
            {
                if (ModelState.IsValid && ShoperLogic.Logon(model.UserName, model.Password, out shopId))
                {
                    Session["ShopId"] = shopId;
                    Session["UserLoginName"] = model.UserName;
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

        [Auth(Roles = "店主")]
        [HttpGet]
        public ActionResult LogOff()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
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

        /// <summary>
        /// 验证码的校验
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ActionResult CheckCode()
        {
            //生成验证码
            ValidateCode validateCode = new ValidateCode();
            string code = validateCode.CreateValidateCode(4);
            Session["ValidateCode"] = code;
            byte[] bytes = validateCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
    }
}