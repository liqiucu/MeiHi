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
        public string RegionName { get; set; }
        public int RegionId { get; set; }

        public List<SelectListItem> RegionNameList
        {
            get;
            set;
        }

        public string Title { get; set; }
        public string Coordinates { get; set; }
        public string DetailAddress { get; set; }
        public string Phone { get; set; }
        public string Contract { get; set; }
        public bool IsHot { get; set; }
        public bool IsOnline { get; set; }
        public string Comment { get; set; }
        public string ParentShopName { get; set; }
        public string ProductBrandId { get; set; }
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
        public string Comment { get; set; }
    }
}