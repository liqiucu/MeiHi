
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
            ViewBag.FirstLogin = true;
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
        //[Auth(RoleName = "管理员")]
        public ActionResult ManageAccount(int? page)
        {
            AccountManageModel amm = new AccountManageModel();
            amm.Admins = manager.GetAdmins(page ?? 1, pagesize);

            return View(amm);
        }

        //[HttpGet]
        //public ActionResult EditAccount(int adminId)
        //{

        //}

        [HttpGet]
        //[Auth(RoleName = "管理员")]
        public ActionResult CreateAccount()
        {
            using (var db = new MeiHiEntities())
            {
                CreateAdminModel adminModel = new CreateAdminModel();
                var roles = db.Role.Where(a => a.Name != "管理员");

                List<SelectListItem> listRoles = new List<SelectListItem>();
                foreach (var item in db.Role)
                {
                    SelectListItem temp = new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.RoleId.ToString()
                    };
                    listRoles.Add(temp);
                }

                List<SelectListItem> listPermissions = new List<SelectListItem>();
                foreach (var item in db.Permission)
                {
                    SelectListItem temp = new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.PermissionId.ToString()
                    };
                    listPermissions.Add(temp);
                }

                adminModel.Permissions = listPermissions;
                adminModel.Roles = listRoles;

                return View(adminModel);
            }
        }

        [HttpPost]
        //[Auth(RoleName = "管理员")]
        public ActionResult SaveAccount(CreateAdminModel adminModel, string[] Role, string[] Permission)
        {
            using (var db = new MeiHiEntities())
            {
                var admin = new MeiHi.Model.Admin()
                {
                    Mobile = adminModel.Mobile,
                    Avaliable = adminModel.Avaliable,
                    Password = adminModel.Password,
                    UserName = adminModel.UserName,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                };

                db.Admin.Add(admin);
                db.SaveChanges();
                var adminId = AdminLogic.GetAdminIdFromAdminName(adminModel.UserName);

                if (Permission != null)
                {
                    foreach (var item in Permission)
                    {
                        AdminPermission adminpermission = new AdminPermission()
                        {
                            AdminId = adminId,
                            Avaliable = true,
                            PermissionId = int.Parse(item),
                            DateCreated = DateTime.Now
                        };

                        db.AdminPermission.Add(adminpermission);
                    }
                }

                if (Role != null)
                {
                    foreach (var item in Role)
                    {
                        AdminRole adminRole = new AdminRole()
                        {
                            AdminId = adminId,
                            RoleId = int.Parse(item),
                            DateCreated = DateTime.Now
                        };

                        db.AdminRole.Add(adminRole);
                    }
                }

                db.SaveChanges();

                return RedirectToAction("ManageAccount", "Account");
            }
        }
    }
}