using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace MeiHi.Admin.Models
{
    public class ShopModel
    {
        public StaticPagedList<ShopListDetailModel> Lists { get; set; }
    }

    public class RegionModel
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; }
    }

    public class CreateShopMpdel
    {
        public int RegionId { get; set; }

        [Display(Name = "区域街道")]
        public List<SelectListItem> RegionNameList
        {
            get;
            set;
        }
        [Display(Name = "店名")]
        public string Title { get; set; }
        [Display(Name = "坐标")]
        public string Coordinates { get; set; }
        [Display(Name = "详细地址")]
        public string DetailAddress { get; set; }
        [Display(Name = "电话")]
        public string Phone { get; set; }
        [Display(Name = "联系人")]
        public string Contract { get; set; }
        [Display(Name="是否热门推荐")]
        public bool IsHot { get; set; }
        [Display(Name = "是否上线")]
        public bool IsOnline { get; set; }
        [Display(Name = "备注")]
        public string Comment { get; set; }
        [Display(Name = "父店名（必须一致）")]
        public string ParentShopName { get; set; }
        [Display(Name = "购买须知")]
        public string PurchaseNotes { get; set; }
        [Display(Name = "店铺Tag")]
        public string ShopTag { get; set; }
    }

    public class EditShopMpdel
    {
        [Display(Name = "店铺Id")]
        public long ShopId { get; set; }
        public int RegionId { get; set; }
         [Display(Name = "区域街道")]
        public List<SelectListItem> RegionNameList
        {
            get;
            set;
        }

        [Display(Name = "店名")]
        public string Title { get; set; }
        [Display(Name = "坐标")]
        public string Coordinates { get; set; }
        [Display(Name = "详细地址")]
        public string DetailAddress { get; set; }
        [Display(Name = "电话")]
        public string Phone { get; set; }
        [Display(Name = "联系人")]
        public string Contract { get; set; }
        [Display(Name = "是否热门推荐")]
        public bool IsHot { get; set; }
        [Display(Name = "是否上线")]
        public bool IsOnline { get; set; }
        [Display(Name = "备注")]
        public string Comment { get; set; }
        [Display(Name = "父店名（必须一致）")]
        public string ParentShopName { get; set; }
        [Display(Name = "购买须知")]
        public string PurchaseNotes { get; set; }
        [Display(Name = "店铺Tag")]
        public string ShopTag { get; set; }

        [Display(Name = "品牌")]
        public List<string> ProductBrandList { get; set; }
        [Display(Name = "门面")]
        public List<string> ShopProductList { get; set; }
    }
    public class ShopDetailMpdel
    {
        public long ShopId { get; set; }
        public string RegionName { get; set; }

        public string Title { get; set; }

        public string Coordinates { get; set; }

        public string DetailAddress { get; set; }

        public string Phone { get; set; }

        public string Contract { get; set; }

        public bool IsHot { get; set; }

        public bool IsOnline { get; set; }

        public string Comment { get; set; }

        public string ParentShopName { get; set; }

        public List<string> ProductBrandList { get; set; }
        public List<string> ShopProductList { get; set; }
        public string PurchaseNotes { get; set; }
        public string ShopTag { get; set; }
    }

    public class ShopListDetailModel
    {
        public long ShopId { get; set; }
        public string DetailAddress { get; set; }
        public string RegionName  { get; set; }
        public string Phone { get; set; }
        public string Contract { get; set; }
        public bool IsHot { get; set; }
        public bool IsOnline { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
    }
}