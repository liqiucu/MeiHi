using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeiHi.Admin.Models.City
{
    public class RegionStreetModel
    {
        [Display(Name = "区域Id")]
        public int RegionId { get; set; }

        [Display(Name = "街道")]
        public List<SelectListItem> StreetList
        {
            get;
            set;
        }
    }
}