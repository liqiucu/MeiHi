using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using MeiHi.Model;
using MeiHi.Shop.Logic;
using MeiHi.CommonDll.Helper;

namespace MeiHi.Shop
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["ShopId"] != null
                && filterContext.HttpContext.Session["UserLoginName"] != null)
            {
                long shopId = (long)filterContext.HttpContext.Session["ShopId"];
                var userLoginName = filterContext.HttpContext.Session["UserLoginName"];

                if (!ShoperLogic.HasPermission(shopId, userLoginName.ToString()) && !filterContext.HttpContext.Response.IsRequestBeingRedirected)
                {
                    filterContext.Result = new RedirectResult(Constants.ShoperLoginUrl);
                    filterContext.HttpContext.Session["permissionnotenough"] = true;
                    filterContext.HttpContext.Session["ReturnUrl"] = filterContext.HttpContext.Request.Url.AbsoluteUri;
                    return;
                }
            }
            else
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JavaScriptResult() { Script = "window.location = '" + Constants.ShoperLoginUrl + "'" };
                    filterContext.HttpContext.Session["ReturnUrl"] = filterContext.HttpContext.Request.Url.AbsoluteUri;
                    return;
                }
                else
                {
                    if (!filterContext.HttpContext.Response.IsRequestBeingRedirected)
                    {
                        filterContext.Result = new RedirectResult(Constants.ShoperLoginUrl);
                        filterContext.HttpContext.Session["ReturnUrl"] = filterContext.HttpContext.Request.Url.AbsoluteUri;
                        return;
                    }
                }
            }
        }
    }
}
