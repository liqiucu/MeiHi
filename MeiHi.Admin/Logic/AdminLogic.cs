using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using MeiHi.Model;
using PagedList;
using MeiHi.Admin.ViewModels;

namespace MeiHi.Admin.Logic
{
    public class AdminLogic
    {
        public static bool HasRole(int adminId, string roleName)
        {
            using (MeiHiEntities db = new MeiHiEntities())
            {
                var admin = db.Admin.FirstOrDefault(a => a.AdminId == adminId);

                if (admin == null)
                {
                    return false;
                }

                var role = db.Role.FirstOrDefault(a => a.Name == roleName);

                if (role == null)
                {
                    return false;
                }

                return admin.AdminRole.FirstOrDefault(a => a.RoleId == role.RoleId) != null;
            }
        }

        public static bool HasPermission(int adminId, string permissionName)
        {
            using (MeiHiEntities db = new MeiHiEntities())
            {
                var admin = db.Admin.FirstOrDefault(a => a.AdminId == adminId);
                if (admin == null)
                {
                    return false;
                }

                var permission = db.Permission.FirstOrDefault(a => a.Name == permissionName);

                if (permission == null)
                {
                    return false;
                }

                var roles = admin.AdminRole;

                if (roles != null)
                {
                    foreach (var item in roles)
                    {
                        var temp = db.RolePermission.Where(a => a.RoleId == item.RoleId);

                        if (temp != null && temp.Count() > 0 && temp.FirstOrDefault(a => a.Permission.Name == permissionName) !=null)
                        {
                            return true;
                        }
                    }
                }

                var permissions = admin.AdminPermission;

                if (permissions != null && permissions.FirstOrDefault(a => a.Permission.Name == permissionName)!=null)
                {
                    return true;
                }

                return false;
            }
        }

        public static int GetAdminIdFromAdminName(string adminName)
        {
            using (var db = new MeiHiEntities())
            {
                if (string.IsNullOrEmpty(adminName))
                {
                    return 0;
                }

                return db.Admin.Where(a => a.UserName == adminName).FirstOrDefault().AdminId;
            }
        }

        public static StaticPagedList<AdminModel> GetAdmins(int page, int pageSize)
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

        public static void DeleteRoleTree(int rootRoleId)
        {
            using (var db = new MeiHiEntities())
            {
                var role = db.Role.FirstOrDefault(a => a.RoleId == rootRoleId);

                if (role == null)
                {
                    return;
                }

                db.AdminRole.RemoveRange(role.AdminRole);
                db.RolePermission.RemoveRange(role.RolePermission);
                db.Role.Remove(role);
                db.SaveChanges();
                var childs = db.Role.Where(a => a.ParentRoleId == rootRoleId);

                if (childs != null)
                {
                    foreach (var item in childs)
                    {
                        DeleteRoleTree(item.RoleId);
                    }
                }


            }
        }

        public static bool Logon(string userName, string password, out int adminId)
        {
            adminId = 0;

            using (MeiHiEntities db = new MeiHiEntities())
            {
                var admin = db.Admin.SingleOrDefault(r => r.Mobile == userName && r.Password == password);
                if (admin != null)
                {
                    adminId = admin.AdminId;
                    return true;
                }
                return false;
            }
        }
    }
}