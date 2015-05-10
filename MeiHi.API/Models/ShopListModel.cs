using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;

namespace MeiHi.API.Models
{
    public class ShopListModel
    {
        MeiHiEntities db = new MeiHiEntities();
        public long ShopId { get; set; }
        public int RegionId { get; set; }
        public string ShopTitle { get; set; }
        public string RegionName
        {
            get
            {
                if (this.RegionId != 0)
                {
                    var region = (from a in db.Region where a.RegionId == this.RegionId select a).FirstOrDefault();

                    if (region == null)
                    {
                        throw new Exception("店铺ID: " + this.ShopId + " 区域未设置");
                    }
                    return region.Name;
                }
                return "";
            }
        }

        /// <summary>
        /// 店铺图片地址
        /// </summary>
        public string ShopImageUrl
        {
            get
            {
                if (this.ProductBrandId == 0)
                {
                    return "";
                }
                var temp = db.ProductBrand.Where(a => a.ProductBrandId == this.ProductBrandId).FirstOrDefault();
                if (temp != null)
                {
                    return temp.ProductUrl;
                }
                return "";
            }
        }

        /// <summary>
        /// 产品品牌ID
        /// </summary>
        public long ProductBrandId { get; set; }

        /// <summary>
        /// 坐标
        /// </summary>
        public string Coordinates { get; set; }

        /// <summary>
        /// 实时计算折扣
        /// </summary>
        public decimal DiscountRate
        {
            get
            {
                if (this.ShopId == 0)
                {
                    return 10;
                }
                var services = db.Service.Where(a => a.ShopId == this.ShopId);
                if (services != null && services.Count() > 0)
                {
                    var lowestDiscountRate = services.OrderBy(a => a.CMUnitCost / a.OriginalUnitCost).FirstOrDefault();

                    if (lowestDiscountRate != null)
                    {
                        var temp = (lowestDiscountRate.CMUnitCost / lowestDiscountRate.OriginalUnitCost);
                        temp = temp * 10;
                        return decimal.Round(temp, 1);
                    }
                }
                return 10;
            }
        }

        /// <summary>
        /// 评分
        /// </summary>
        public decimal Rate
        {
            get
            {
                if (this.ShopId == 0)
                {
                    return 4.5M;
                }

                var temp = db.UserComments.Where(a => a.ShopId == this.ShopId);

                if (temp != null && temp.Count() > 0)
                {
                    return decimal.Round(temp.Sum(a => a.Rate) / temp.Count(), 1);
                }

                return 4.5M;
            }
        }

        /// <summary>
        /// 实时计算距离
        /// </summary>
        public int Distance { get; set; }
    }
}