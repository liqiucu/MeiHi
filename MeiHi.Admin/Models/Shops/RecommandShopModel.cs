using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models
{
    public class RecommandShopModel
    {
        [Display(Name = "店铺ID")]
        public long ShopId { get; set; }

        [Display(Name = "店铺名字")]
        public string ShopName { get; set; }
        [Display(Name = "区域")]
        public string Region { get; set; }
        [Display(Name = "最后修改时间")]
        public DateTime LastModifyTime { get; set; }
    }
}