using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using System.Web.Mvc;

namespace MeiHi.Admin.Logic
{
    public class CommonLogic
    {
        public List<SelectListItem> RegionList()
        {
            using (var access = new MeiHiEntities())
            {
                List<SelectListItem> Lists = new List<SelectListItem>();

                access.Region.ToList().ForEach((item) =>
                {
                    Lists.Add(new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.RegionId.ToString()
                    });
                });

                return Lists;
            }
        }
    }
}