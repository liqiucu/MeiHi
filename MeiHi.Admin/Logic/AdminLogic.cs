using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using MeiHi.Admin.Helper;
using MeiHi.Model;

namespace MeiHi.Admin.Logic
{
    public class AdminLogic
    {
        public static bool HasRole(int? adminId, string roleName)
        {
            using (MeiHiEntities db = new MeiHiEntities())
            {
                return false;
                //return db.sp_admin_has_role(adminId, roleName).First().Value;
            }
        }

        public static bool HasPermission(int? adminId, string permissionName)
        {
            using (MeiHiEntities db = new MeiHiEntities())
            {
                return false;
               // return db.sp_admin_has_permission(adminId, permissionName).First().Value;
            }
        }


        public static bool Logon(string userName, string password, out int adminId)
        {
            adminId = 0;

            using (MeiHiEntities db = new MeiHiEntities())
            {
                var admin = db.Admin.SingleOrDefault(r => r.UserName == userName && r.Password==password);
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