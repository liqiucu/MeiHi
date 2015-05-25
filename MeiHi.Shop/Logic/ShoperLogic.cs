using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using MeiHi.Model;

namespace MeiHi.Shop.Logic
{
    public class ShoperLogic
    {
        public static bool HasPermission(long shopId)
        {
            using (var db = new MeiHiEntities())
            {
                var shopuser = db.ShopUser.FirstOrDefault(a => a.ShopId == shopId);

                if (shopuser == null)
                {
                    return false;
                }

                return true;
            }
        }

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

        public static bool Logon(string userName, string password, out long shopId)
        {
            shopId = 0;

            using (MeiHiEntities db = new MeiHiEntities())
            {
                var shoper = db.ShopUser.SingleOrDefault(r => r.ShopUserName == userName && r.Password == password);
                if (shoper != null)
                {
                    shopId = shoper.ShopId;
                    return true;
                }
                return false;
            }
        }
    }
}