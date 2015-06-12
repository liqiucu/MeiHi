using MeiHi.Admin.Logic;
using MeiHi.Admin.Models.Statistics;
using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeiHi.Admin.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult SearchUserStatistics(DateTime? start = null, DateTime? end = null)
        {
            using (var db = new MeiHiEntities())
            {
                var model = StatisticsLogic.Statistics(start, end);

                if (start != null && end != null)
                {
                    TempData["message"] = "搜索起始：" + start + " 结束：" + end;
                    TempData["start"] = start;
                    TempData["end"] = end;
                }


                return View(model);
            }
        }
    }
}
