using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using System.Web.Mvc;

namespace MeiHi.Admin.Logic
{
    public static class ServiceLogic
    {
        public static List<SelectListItem> ServiceTypeList(int serviceTypeId = 0)
        {
            using (var access = new MeiHiEntities())
            {
                List<SelectListItem> Lists = new List<SelectListItem>();

                access.ServiceType.ToList().ForEach((item) =>
                {
                    Lists.Add(new SelectListItem()
                    {
                        Text = item.Title,
                        Value = item.ServiceTypeId.ToString(),
                        Selected = item.ServiceTypeId == serviceTypeId
                    });
                });

                return Lists;
            }
        }
    }
}