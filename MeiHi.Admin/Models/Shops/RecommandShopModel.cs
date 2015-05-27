using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models
{
    public class RecommandShopModel
    {
        public long ShopId { get; set; }

        public string ShopName { get; set; }

        public string Region { get; set; }

        public DateTime LastModifyTime { get; set; }
    }
}