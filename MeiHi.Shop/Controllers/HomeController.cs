using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeiHi.Model;

namespace MeiHi.Shop.Controllers
{
    public class HomeController : Controller
    {
        [Auth(Roles = "店主")]
        public ActionResult Index()
        {

            return View();
        }

        [Auth(Roles = "店主")]
        [HttpPost]
        public ActionResult VerifyUserMeiHiCode(string code)
        {
            using (var db = new MeiHiEntities())
            {
                var booking = db.Booking.FirstOrDefault(a => a.VerifyCode == code && !a.IsUsed && a.IsBilling);

                if (booking != null)
                {
                    booking.IsUsed = true;
                    db.SaveChanges();
                    ViewBag.Verified = true;
                }

                return View("Index");
            }
        }
    }
}