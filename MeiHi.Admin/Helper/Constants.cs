using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiHi.Admin.Helper
{
    public static class Constants
    {
        public const string DateFormat = "yyyy-MM-dd";
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public static class AdminSite
        {
            public const string AdminUrl = "http://localhost:37628/";
            public const string AdminLoginUrl = "http://localhost:37628/account/login";
            public const string AdminLogoutUrl = "http://localhost:37628/account/logout";
        }
    }
}
