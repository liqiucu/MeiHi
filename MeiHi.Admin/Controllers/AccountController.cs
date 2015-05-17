
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;
using MeiHi.Admin.Logic;
using MeiHi.Admin.ViewModels;
using System.Web.Security;

namespace MeiHi.Admin.Controllers
{
    public class AccountController : Controller
    {
        AdminAccountManager manager = new AdminAccountManager();
        int pagesize = 10;

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            int adminId = 0;

            if (ModelState.IsValid && AdminLogic.Logon(model.UserName, model.Password, out adminId))
            {
                Session["AdminId"] = adminId;
                return RedirectToAction("Shop", "ShopManege");
            }

            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult ManageAccount(int? page)
        {
            AccountManageModel amm = new AccountManageModel();
            amm.ListAdmins = manager.GetAdmins(page ?? 1, pagesize);
            amm.ListRoles = manager.GetAdminRoles();

            return View(amm);
        }

        [HttpPost]
        [Auth(RoleName = "管理员")]
        public ActionResult CreateAccount(AdminModel adminModel)
        {
            int adminId = manager.CreateAdminAccount(new MeiHi.Model.Admin()
            {
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                UserName = adminModel.UserName,
                Password = adminModel.Password
            });

            int roleId = int.Parse(adminModel.Role);
            manager.CreatAdminRole(adminId, roleId);

            return View();
        }

        [HttpPost]
        [Auth(RoleName = "管理员")]
        public JsonResult ChangeAccount(AdminModel adminModel)
        {
            manager.UpdateAdminAccount(adminModel);

            return Json("update account success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Auth(RoleName = "管理员")]
        public JsonResult DeleteAdminAccount(string UserId)
        {
            manager.DeleteAdminAccount(int.Parse(UserId));

            return Json("delete account success", JsonRequestBehavior.AllowGet);
        }
    }
}