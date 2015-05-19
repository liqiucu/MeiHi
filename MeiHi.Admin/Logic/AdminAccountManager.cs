using MeiHi.Admin.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using PagedList;
using MeiHi.Admin.ViewModels;
using MeiHi.Model;

namespace MeiHi.Admin.Logic
{
    public class AdminAccountManager
    {
        public bool CheckAdminLogininfo(LoginModel loginModel)
        {
            using (var access = new MeiHiEntities())
            {
                var result = from a in access.Admin
                             where a.UserName == loginModel.UserName
                             && a.Password == loginModel.Password
                             select a;
                bool validate = result.Any();
                if (validate)
                {
                    HttpContext.Current.Session["AdminId"] = result.FirstOrDefault().AdminId;
                    FormsAuthentication.SetAuthCookie(loginModel.UserName, false);
                }
                return validate;
            }
        }

        public int CreateAdminAccount(MeiHi.Model.Admin admin)
        {
            using (var access = new MeiHiEntities())
            {
                MeiHi.Model.Admin user = access.Admin.Add(admin);
                access.SaveChanges();
                return user.AdminId;
            }
        }

        public void CreatAdminRole(int adminId, int roleId)
        {
            using (var access = new MeiHiEntities())
            {
                access.AdminRole.Add(new AdminRole()
                {
                    AdminId = adminId,
                    RoleId = roleId,
                    DateCreated = DateTime.Now
                });

                access.SaveChanges();
            }
        }

        public List<RoleModel> GetAdminRoles()
        {
            List<RoleModel> result = new List<RoleModel>();

            using (var access = new MeiHiEntities())
            {
                access.Role.ToList().ForEach((item) =>
                {
                    result.Add(new RoleModel() { RoleId = item.RoleId, RoleName = item.Name });
                });
            }


            return result;
        }

        public StaticPagedList<AdminModel> GetAdmins(int page, int pageSize)
        {
            using (var access = new MeiHiEntities())
            {
                var admins = access.Admin.OrderBy(a => a.AdminId).Skip((page - 1) * pageSize).Take(pageSize);//.Where(a => a.AdminRole.FirstOrDefault(b => b.Role.Name == "管理员") == null)
                List<AdminModel> temp = new List<AdminModel>();

                if (admins != null)
                {
                    foreach (var item in admins)
                    {
                        temp.Add(new AdminModel()
                        {
                            AdminId = item.AdminId,
                            UserName = item.UserName,
                            Mobile = item.Mobile,
                            Avaliable = item.Avaliable,
                            PermissionNames = item.AdminPermission != null ? item.AdminPermission.Select(a => a.Permission.Name).ToList() : null,
                            RoleNmes = item.AdminRole != null ? item.AdminRole.Select(a => a.Role.Name).ToList() : null
                        });
                    }
                }

                StaticPagedList<AdminModel> result = new StaticPagedList<AdminModel>(temp, page, pageSize, access.Admin.Count());

                return result;
            }
        }

        private string GetRoleNameByAdminId(int adminId)
        {
            using (var access = new MeiHiEntities())
            {
                var adminRole = access.AdminRole.SingleOrDefault(a => a.AdminId == adminId);
                return adminRole != null ? adminRole.Role.Name : null;
            }
        }

        //public void UpdateAdminAccount(AdminModel adminModel)
        //{
        //    using (var access = new MeiHiEntities())
        //    {
        //        var admin = access.Admin.FirstOrDefault(a => a.AdminId == adminModel.UserId);

        //        if (admin != null)
        //        {
        //            admin.UserName = adminModel.UserName;
        //            if (IsAccountPasswordChange(adminModel.UserId, adminModel.Password))
        //            {
        //                // admin.Password = Helper.GenerateHashWithSalt(adminModel.Password, admin.Salt);
        //            }
        //        }

        //        access.SaveChanges();
        //        // EF不允许更新主键，所以adminrole全删了，再插入
        //        access.AdminRole.Where(a => a.AdminId == adminModel.UserId).ToList().ForEach(r => access.AdminRole.Remove(r));
        //        CreatAdminRole(adminModel.UserId, int.Parse(adminModel.Role));
        //        access.SaveChanges();
        //    }
        //}

        public void DeleteAdminAccount(int userId)
        {
            using (var access = new MeiHiEntities())
            {
                var admins = access.Admin.Where(a => a.AdminId == userId).FirstOrDefault();

                if (admins != null)
                {
                    access.AdminRole.RemoveRange(admins.AdminRole);
                    access.AdminPermission.RemoveRange(admins.AdminPermission);
                    access.Admin.Remove(admins);
                    access.SaveChanges();
                }
            }
        }

        private bool IsAccountPasswordChange(int adminId, string encryptedPassword)
        {
            using (var access = new MeiHiEntities())
            {
                var admin = access.Admin.Where(a => a.AdminId == adminId);

                if (admin != null)
                {
                    for (int i = 0; i < encryptedPassword.Length; i++)
                    {
                        if (encryptedPassword[i] != admin.FirstOrDefault().Password[i])
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}