using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models
{
    public class ProductBrandImages
    {
        MeiHiEntities db = new MeiHiEntities();
        public long ShopId { get; set; }

        /// <summary>
        /// 用户点击店铺图标的时候 需要展示图片列表
        /// </summary>
        public List<string> ProductBrandUrls
        {
            get;
            set;
        }
    }
}