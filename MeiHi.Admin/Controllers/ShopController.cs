using MeiHi.Admin.Models;
using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using MeiHi.Admin.Models;

namespace MeiHi.Admin.Controllers
{
    public class ShopController : Controller
    {
        MeiHiEntities db = new MeiHiEntities();

        /// <summary>
        /// 创建店铺后出现图片地址编辑
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        public ActionResult CreateShop(MeiHi.Admin.Models.ShopModel shop)
        {
            return View();
        }
    }
}