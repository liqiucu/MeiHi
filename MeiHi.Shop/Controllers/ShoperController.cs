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
using PagedList.Mvc;
using PagedList;
using Owin;
using System.Data.Entity.Validation;
using System.IO;
using MeiHi.CommonDll.Helper;
using MeiHi.Shop.Logic;
using MeiHi.Shop.Models.Shoper;
using MeiHi.Shop.Models.User;

namespace MeiHi.Shop.Controllers
{
    public class ShoperController : Controller
    {
        // GET: Shoper
        public ActionResult ManageShopers(int page = 1)
        {
            var model = new ShopersModel()
            {
                Shopers = ShoperLogic.GetShopers(page, 10)
            };

            return View(model);
        }

        public ActionResult GetUnBillingBookingsByShopId(long shopId, int page = 1)
        {
            return View(ShoperLogic.GetUnBillingBookingsByShopId(shopId, page, 10));
        }

        public ActionResult GetBillingedBookingsByShopId(long shopId, int page = 1)
        {
            return View(ShoperLogic.GetBillingedBookingsByShopId(shopId, page, 10));
        }

        public ActionResult ManageUserCommentsByShopId(long shopId, int page = 1)
        {
            return View(ShoperLogic.GetUserCommentsByShopId(shopId, page, 10));
        }
    }
}