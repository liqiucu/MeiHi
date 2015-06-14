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
        public List<SelectListItem> RegionList(int regionId = 0, int cityId = 20)
        {
            using (var access = new MeiHiEntities())
            {
                List<SelectListItem> Lists = new List<SelectListItem>();

                access.Region.Where(a => a.ParentRegionId == cityId).ToList().ForEach((item) =>
                {
                    Lists.Add(new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.RegionId.ToString(),
                        Selected = item.RegionId == regionId
                    });
                });

                return Lists;
            }
        }

        public List<SelectListItem> StreetList(int regionId, int streetId = 0)
        {
            using (var access = new MeiHiEntities())
            {
                List<SelectListItem> Lists = new List<SelectListItem>();
                var temp = access.Region.Where(a => a.ParentRegionId == regionId);

                Lists.Add(new SelectListItem()
                {
                    Text = "可空",
                    Value = "0"
                });

                if (temp != null && temp.Count() > 0)
                {
                    
                    temp.ToList().ForEach((item) =>
                    {
                        Lists.Add(new SelectListItem()
                        {
                            Text = item.Name,
                            Value = item.RegionId.ToString(),
                            Selected = item.RegionId == streetId
                        });
                    });
                }

                return Lists;
            }
        }
    }
}