
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

        [HttpPost]
        public ActionResult UpdateAccount(EditAdminModel model, string[] Role, string[] Permission)
        {
            using (var db = new MeiHiEntities())
            {
                var account = db.Admin.FirstOrDefault(a => a.AdminId == model.AdminId);

                if (account == null)
                {
                    throw new Exception("账号不存在");
                }

                account.Avaliable = model.Avaliable;
                account.UserName = model.UserName;
                account.Mobile = model.Mobile;
                account.Password = model.Password;
                account.DateModified = DateTime.Now;
                db.AdminPermission.RemoveRange(account.AdminPermission);
                db.AdminRole.RemoveRange(account.AdminRole);
                db.SaveChanges();

                if (Permission != null)
                {
                    foreach (var item in Permission)
                    {
                        AdminPermission adminpermission = new AdminPermission()
                        {
                            AdminId = model.AdminId,
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
                            AdminId = model.AdminId,
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

        [HttpGet]
        public ActionResult DeleteAccount(int adminId)
        {
            using (var db = new MeiHiEntities())
            {
                var account = db.Admin.FirstOrDefault(a => a.AdminId == adminId);

                if (account == null)
                {
                    throw new Exception("账号不存在");
                }

                db.AdminPermission.RemoveRange(account.AdminPermission);
                db.AdminRole.RemoveRange(account.AdminRole);
                db.Admin.Remove(account);
                db.SaveChanges();
                return RedirectToAction("ManageAccount", "Account");
            }
        }

        [HttpGet]
        public ActionResult AccountDetail(int adminId)
        {
            using (var db = new MeiHiEntities())
            {
                var account = db.Admin.FirstOrDefault(a => a.AdminId == adminId);

                if (account == null)
                {
                    throw new Exception("账号不存在");
                }

                AdminModel adminModel = new AdminModel()
                {
                    AdminId = account.AdminId,
                    UserName = account.UserName,
                    Mobile = account.Mobile,
                    Password = account.Password,
                    Avaliable = account.Avaliable,
                    PermissionNames = account.AdminPermission != null ? account.AdminPermission.Select(a => a.Permission.Name).ToList() : null,
                    RoleNmes = account.AdminRole != null ? account.AdminRole.Select(a => a.Role.Name).ToList() : null
                };

                return View(adminModel);
            }
        }

        [HttpGet]
        public ActionResult EditAccount(int adminId)
        {
            using (var db = new MeiHiEntities())
            {
                var account = db.Admin.FirstOrDefault(a => a.AdminId == adminId);

                if (account == null)
                {
                    throw new Exception("账号不存在，可能是数据库异常了或者网络异常了");
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

                if (account.AdminPermission != null && account.AdminPermission.Count > 0)
                {
                    foreach (var item in account.AdminPermission.Select(a => a.Permission))
                    {
                        listPermissions.First(a => a.Value == item.PermissionId.ToString()).Selected = true;
                    }
                }

                List<SelectListItem> listRoles = new List<SelectListItem>();

                foreach (var item in db.Role.Where(a => a.Name != "管理员"))
                {
                    SelectListItem temp = new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.RoleId.ToString()
                    };

                    listRoles.Add(temp);
                }

                if (account.AdminRole != null && account.AdminRole.Count > 0)
                {
                    foreach (var item in account.AdminRole.Select(a => a.Role))
                    {
                        listRoles.First(a => a.Value == item.RoleId.ToString()).Selected = true;
                    }
                }

                EditAdminModel adminModel = new EditAdminModel()
                {
                    AdminId = adminId,
                    Avaliable = account.Avaliable,
                    Mobile = account.Mobile,
                    Password = account.Password,
                    UserName = account.UserName,
                    Permissions = listPermissions,
                    Roles = listRoles
                };

                return View(adminModel);
            }
        }

        [HttpGet]
        //[Auth(RoleName = "管理员")]
        public ActionResult CreateAccount()
        {
            using (var db = new MeiHiEntities())
            {
                CreateAdminModel adminModel = new CreateAdminModel();
                var roles = db.Role.Where(a => a.Name != "管理员");

                List<SelectListItem> listRoles = new List<SelectListItem>();
                foreach (var item in roles)
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