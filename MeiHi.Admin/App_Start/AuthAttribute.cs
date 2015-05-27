using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using MeiHi.Model;
using MeiHi.Admin.Logic;
using MeiHi.CommonDll.Helper;

namespace MeiHi.Admin
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthAttribute : AuthorizeAttribute
    {
        public string RoleName { get; set; }
        public string PermissionName { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["AdminId"] != null)
            {
                bool hasPermission = false;
                int adminId = (int)filterContext.HttpContext.Session["AdminId"];

                if (!string.IsNullOrEmpty(RoleName) && !string.IsNullOrEmpty(PermissionName))
                {
                    hasPermission = AdminLogic.HasRole(adminId, RoleName) && AdminLogic.HasPermission(adminId, PermissionName);
                }
                else if (!string.IsNullOrEmpty(RoleName))
                {
                    hasPermission = AdminLogic.HasRole(adminId, RoleName);
                }
                else if (!string.IsNullOrEmpty(PermissionName))
                {
                    hasPermission = AdminLogic.HasPermission(adminId, PermissionName);
                }
                else
                {
                    hasPermission = true;
                }

                if (!hasPermission)
                {
                    if (!filterContext.HttpContext.Response.IsRequestBeingRedirected)
                    {
                        filterContext.Result = new RedirectResult(Constants.AdminLoginUrl);
                        filterContext.HttpContext.Session["permissionnotenough"] = true;
                        filterContext.HttpContext.Session["ReturnUrl"] = filterContext.HttpContext.Request.Url.AbsoluteUri;
                        return;
                    }
                }
            }
            else
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JavaScriptResult() { Script = "window.location = '" + Constants.AdminLoginUrl + "'" };
                    filterContext.HttpContext.Session["ReturnUrl"] = filterContext.HttpContext.Request.Url.AbsoluteUri;
                    return;
                }
                else
                {
                    if (!filterContext.HttpContext.Response.IsRequestBeingRedirected)
                    {
                        filterContext.Result = new RedirectResult(Constants.AdminLoginUrl);
                        filterContext.HttpContext.Session["ReturnUrl"] = filterContext.HttpContext.Request.Url.AbsoluteUri;
                        return;
                    }
                }
            }
        }
    }
}
