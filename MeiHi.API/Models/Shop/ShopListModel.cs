//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using MeiHi.Model;

//namespace MeiHi.API.Models
//{
//    public class ShopListModel
//    {
//        MeiHiEntities db = new MeiHiEntities();
//        public long ShopId { get; set; }
//        public int RegionId { get; set; }
//        public string ShopTitle { get; set; }
//        public string RegionName
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 店铺图片地址
//        /// </summary>
//        public string ShopImageUrl
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 产品品牌ID
//        /// </summary>
//        public string ProductBrandId { get; set; }

//        /// <summary>
//        /// 坐标
//        /// </summary>
//        public string Coordinates { get; set; }

//        /// <summary>
//        /// 实时计算折扣
//        /// </summary>
//        public decimal DiscountRate
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 评分
//        /// </summary>
//        public decimal Rate
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 实时计算距离
//        /// </summary>
//        public int Distance { get; set; }
//    }
//}