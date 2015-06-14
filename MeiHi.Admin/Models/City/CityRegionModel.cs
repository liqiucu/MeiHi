using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace MeiHi.Admin.Models.City
{
    public class CityRegionModel
    {
        [Display(Name = "所属省份ID")]
        public int ProvinceId { get; set; }

        [Display(Name = "所属省份名称")]
        public int ProvinceName { get; set; }

        [Display(Name = "城市Id")]
        public int CityId { get; set; }

        [Display(Name = "城市名称")]
        public string CityName { get; set; }

        [Display(Name = "区域列表")]
        public List<RegionModel> RegionList
        {
            get;
            set;
        }
    }

    public class RegionModel
    {
        [Display(Name = "区域Id")]
        public int RegionId { get; set; }

        [Display(Name = "区域名称")]
        public string RegionName { get; set; }

        [Display(Name = "父区域名称")]
        public string ParentRegionName { get; set; }

        [Display(Name = "父区域ID")]
        public int ParentRegionId { get; set; }

        [Display(Name = "root区域ID")]
        public int RootRegionId { get; set; }

        [Display(Name = "所属省份ID")]
        public int ProvinceId { get; set; }

        [Display(Name = "所属省份名称")]
        public int ProvinceName { get; set; }

        [Display(Name = "街道")]
        public List<StreetModel> StreetList
        {
            get;
            set;
        }
    }

    public class StreetModel
    {
        [Display(Name = "所属省份ID")]
        public int ProvinceId { get; set; }

        [Display(Name = "所属省份名称")]
        public int ProvinceName { get; set; }

        [Display(Name = "街道Id")]
        public int RegionId { get; set; }

        [Display(Name = "街道名称")]
        public string RegionName { get; set; }
    }
}