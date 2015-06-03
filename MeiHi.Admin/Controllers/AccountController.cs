
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;
using MeiHi.Admin.Logic;
using MeiHi.Admin.ViewModels;
using System.Web.Security;
using MeiHi.CommonDll;

namespace MeiHi.Admin.Controllers
{
    public class AccountController : Controller
    {
        AdminAccountManager manager = new AdminAccountManager();
        int pagesize = 10;

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

            int adminId = 0;

            using (var db = new MeiHiEntities())
            {
                if (ModelState.IsValid && AdminLogic.Logon(model.UserName, model.Password, out adminId))
                {
                    Session["AdminId"] = adminId;
                    Session["UserName"] = db.Admin.FirstOrDefault(a => a.AdminId == adminId).UserName;

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

        #region 账号管理

        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult ManageAccount(int? page)
        {
            AccountManageModel amm = new AccountManageModel();
            amm.Admins = manager.GetAdmins(page ?? 1, pagesize);

            return View(amm);
        }

        [HttpPost]
        [Auth(RoleName = "管理员")]
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
        [Auth(RoleName = "管理员")]
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
        [Auth(RoleName = "管理员")]
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
        [Auth(RoleName = "管理员")]
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

                foreach (var item in db.Role)//.Where(a => a.Name != "管理员"))
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
        [Auth(RoleName = "管理员")]
        public ActionResult CreateAccount()
        {
            using (var db = new MeiHiEntities())
            {
                CreateAdminModel adminModel = new CreateAdminModel();
                var roles = db.Role;//.Where(a => a.Name != "管理员");

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
        [Auth(RoleName = "管理员")]
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

        #endregion

        #region 角色管理

        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult RoleManage()
        {
            using (var db = new MeiHiEntities())
            {
                var roles = db.Role;//.Where(a => a.Name != "管理员");
                List<RoleModel> roleModes = new List<RoleModel>();

                foreach (var item in roles)
                {
                    RoleModel roleMode = new RoleModel()
                    {
                        PermissionNames = item.RolePermission != null && item.RolePermission.Count > 0 ? item.RolePermission.Select(a => a.Permission.Name).ToList() : null,
                        RoleId = item.RoleId,
                        RoleName = item.Name,
                        ParentRoleId = item.ParentRoleId,
                        ParentRoleName = db.Role.FirstOrDefault(a => a.RoleId == item.ParentRoleId.Value) != null ? db.Role.FirstOrDefault(a => a.RoleId == item.ParentRoleId.Value).Name : null
                    };

                    roleModes.Add(roleMode);
                }

                return View(roleModes);
            }
        }

        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult CreateRole()
        {
            using (var db = new MeiHiEntities())
            {
                var roles = new List<SelectListItem>();

                foreach (var item in db.Role)
                {
                    roles.Add(new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.RoleId.ToString()
                    });
                }
                var permissions = new List<SelectListItem>();
                foreach (var item in db.Permission)
                {
                    SelectListItem temp = new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.PermissionId.ToString()
                    };
                    permissions.Add(temp);
                }

                CreateRoleModel roleMode = new CreateRoleModel()
                {
                    ParentRoleList = roles,
                    Permissions = permissions
                };

                return View(roleMode);
            }
        }

        [HttpPost]
        [Auth(RoleName = "管理员")]
        public ActionResult SaveRole(CreateRoleModel model, string[] Permission)
        {
            using (var db = new MeiHiEntities())
            {
                var role = new Role()
                {
                    Name = model.RoleName,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    ParentRoleId = model.ParentRoleId
                };

                db.Role.Add(role);
                db.SaveChanges();

                var roleId = db.Role.FirstOrDefault(a => a.Name == model.RoleName).RoleId;

                if (Permission != null)
                {
                    foreach (var item in Permission)
                    {
                        RolePermission rolePermission = new RolePermission()
                        {
                            RoleId = roleId,
                            PermissionId = int.Parse(item),
                            DateCreated = DateTime.Now
                        };

                        db.RolePermission.Add(rolePermission);
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("RoleManage");
            }
        }

        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult DeleteRole(int roleId)
        {
            using (var db = new MeiHiEntities())
            {
                var role = db.Role.FirstOrDefault(a => a.RoleId == roleId);

                if (role == null)
                {
                    throw new Exception("角色不存在");
                }

                AdminLogic.DeleteRoleTree(role.RoleId);

                return RedirectToAction("RoleManage");
            }
        }

        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult EditRole(int roleId)
        {
            using (var db = new MeiHiEntities())
            {
                var role = db.Role.FirstOrDefault(a => a.RoleId == roleId);
                if (role == null)
                {
                    throw new Exception("角色不存在");
                }
                var roles = new List<SelectListItem>();

                foreach (var item in db.Role)
                {
                    roles.Add(new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.RoleId.ToString()
                    });
                }

                if (role.ParentRoleId != null)
                {
                    roles.FirstOrDefault(a => a.Value == role.ParentRoleId.ToString()).Selected = true;
                }

                var permissions = new List<SelectListItem>();

                foreach (var item in db.Permission)
                {
                    SelectListItem temp = new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.PermissionId.ToString()
                    };
                    permissions.Add(temp);
                }

                if (role.RolePermission != null)
                {
                    foreach (var item in role.RolePermission)
                    {
                        permissions.First(a => a.Value == item.PermissionId.ToString()).Selected = true;
                    }
                }

                var editRole = new EditRoleModel()
                {
                    RoleId = roleId,
                    ParentRoleList = roles,
                    Permissions = permissions,
                    RoleName = role.Name
                };

                return View(editRole);
            }
        }

        [HttpPost]
        [Auth(RoleName = "管理员")]
        public ActionResult UpdateRole(EditRoleModel model, string[] Permission)
        {
            using (var db = new MeiHiEntities())
            {
                var role = db.Role.FirstOrDefault(a => a.RoleId == model.RoleId);
                if (role == null)
                {
                    throw new Exception("角色不存在");
                }

                role.Name = model.RoleName;
                role.DateModified = DateTime.Now;
                role.ParentRoleId = model.ParentRoleId;

                if (Permission != null)
                {
                    db.RolePermission.RemoveRange(role.RolePermission);

                    foreach (var item in Permission)
                    {
                        RolePermission rolePermission = new RolePermission()
                        {
                            RoleId = role.RoleId,
                            PermissionId = int.Parse(item),
                            DateCreated = DateTime.Now
                        };

                        db.RolePermission.Add(rolePermission);
                    }
                }

                db.SaveChanges();

                return RedirectToAction("RoleManage");
            }
        }
        #endregion

        #region 权限管理
        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult PermissionManage()
        {
            using (var db = new MeiHiEntities())
            {
                var permissions = db.Permission;
                List<PermissionModel> permissionModes = new List<PermissionModel>();

                foreach (var item in permissions)
                {
                    var roleMode = new PermissionModel()
                    {
                        Description = item.Description,
                        Group = item.Group,
                        PermissionId = item.PermissionId,
                        LastMidifyTime = item.DateModified,
                        PermissionName = item.Name
                    };

                    permissionModes.Add(roleMode);
                }

                return View(permissionModes);
            }
        }

        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult CreatePermission()
        {
            var permission = new PermissionModel();
            return View(permission);
        }

        [HttpPost]
        [Auth(RoleName = "管理员")]
        public ActionResult SavePermission(PermissionModel model)
        {
            using (var db = new MeiHiEntities())
            {
                var permission = new Permission()
                {
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Description = model.Description,
                    Group = model.Group,
                    Name = model.PermissionName
                };

                db.Permission.Add(permission);
                db.SaveChanges();

                return RedirectToAction("PermissionManage");
            }
        }

        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult DeletePermission(int permissionId)
        {
            using (var db = new MeiHiEntities())
            {
                var permission = db.Permission.FirstOrDefault(a => a.PermissionId == permissionId);
                if (permission != null)
                {
                    db.AdminPermission.RemoveRange(permission.AdminPermission);
                    db.RolePermission.RemoveRange(permission.RolePermission);

                    db.Permission.Remove(permission);
                    db.SaveChanges();
                }

                return RedirectToAction("PermissionManage");
            }
        }

        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult EditPermission(int permissionId)
        {
            using (var db = new MeiHiEntities())
            {
                var permission = db.Permission.FirstOrDefault(a => a.PermissionId == permissionId);

                if (permission == null)
                {
                    throw new Exception("权限ID：" + permissionId + " 不存在");
                }

                var model = new PermissionModel()
                {
                    Description = permission.Description,
                    Group = permission.Group,
                    PermissionId = permission.PermissionId,
                    PermissionName = permission.Name
                };
                return View(model);
            }
        }

        [HttpGet]
        [Auth(RoleName = "管理员")]
        public ActionResult UpdatePermission(PermissionModel model)
        {
            using (var db = new MeiHiEntities())
            {
                var permission = db.Permission.FirstOrDefault(a => a.PermissionId == model.PermissionId);

                if (permission != null)
                {
                    permission.DateModified = DateTime.Now;
                    permission.Description = model.Description;
                    permission.Group = model.Group;
                    permission.Name = model.PermissionName;
                    db.SaveChanges();
                }

                return RedirectToAction("PermissionManage");
            }
        }

        [HttpPost]
        [Auth(RoleName = "管理员")]
        public ActionResult UpdateRole(PermissionModel model)
        {
            using (var db = new MeiHiEntities())
            {
                var permission = db.Permission.FirstOrDefault(a => a.PermissionId == model.PermissionId);

                if (permission == null)
                {
                    throw new Exception("权限ID：" + model.PermissionId + " 不存在");
                }

                permission.Group = model.Group;
                permission.Description = model.Description;
                permission.Name = model.PermissionName;
                permission.DateModified = DateTime.Now;
                db.SaveChanges();

                return RedirectToAction("PermissionManage");
            }
        }
        #endregion
    }
}