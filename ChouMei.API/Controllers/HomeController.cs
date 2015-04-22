using ChouMei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChouMei.Controllers
{
    public class HomeController : Controller
    {
        SiTuXiaoYuanEntities test = new SiTuXiaoYuanEntities();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            var tt = test.Admins.Where(a => a.AdminId == 1);
            return View();
        }
    }
}
