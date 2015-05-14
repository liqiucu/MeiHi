using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.API.Models
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
            get
            {
                if (this.ShopId == 0)
                {
                    return null;
                }

                var temp = db.Shop.Where(a => a.ShopId == this.ShopId).FirstOrDefault();

                if (temp != null)
                {
                    var productBrands = db.ProductBrand.Where(a => a.ProductBrandId == temp.ProductBrandId);

                    if (productBrands != null && productBrands.Count() > 0)
                    {
                        return productBrands.Select(a => a.ProductUrl).ToList();
                    }
                }

                return null;
            }
        }
    }
}