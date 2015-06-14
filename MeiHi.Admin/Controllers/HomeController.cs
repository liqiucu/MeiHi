using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Text;

namespace MeiHi.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Auth]
        public ActionResult Index()
        {
            return View();
            //  return RedirectToAction("ShopManege", "Shop");
        }
    }
}