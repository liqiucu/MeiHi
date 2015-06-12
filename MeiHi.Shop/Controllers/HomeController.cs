using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;
using MeiHi.Shop.ViewModels;
using MeiHi.Shop.Models.HomeModel;

namespace MeiHi.Shop.Controllers
{
    public class HomeController : Controller
    {
        [Auth(Roles = "店主")]
        public ActionResult Index()
        {
            if (Session["ShopId"] != null)
            {
                var shopId = long.Parse(Session["ShopId"].ToString());
                return RedirectToAction("ManageBookings", "Booking", new { shopId = shopId });
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}