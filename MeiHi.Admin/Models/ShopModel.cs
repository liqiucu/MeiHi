using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models
{
    public class ShopModel
    {
        public long ShopId { get; set; }
        public string DetailAddress { get; set; }
        public int RegionID { get; set; }
        public Nullable<long> ParentShopId { get; set; }
        public string Phone { get; set; }
        public string Contract { get; set; }
        public bool IsOnline { get; set; }
        public string ProductBrandId { get; set; }
        public string PurchaseNotes { get; set; }
        public string Coordinates { get; set; }
        public bool IsHot { get; set; }
        public string ShopTag { get; set; }
        public string Comment { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
    }
}