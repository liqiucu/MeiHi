using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using System.Runtime.Serialization;

namespace MeiHi.Admin.Models
{
    public class ShopXModel
    {
        MeiHiEntities db = new MeiHiEntities();

        public long ShopId { get; set; }

        public string DetailAddress { get; set; }

        public string RegionName
        {
            get;
            set;
        }
        /// <summary>
        /// 店铺图片地址
        /// </summary>

        public string ShopImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 联系电话
        /// </summary>
  
        public string Phone { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>

        public string Contract { get; set; }

        public string PurchaseNotes { get; set; }
        public decimal Rate { get; set; }
        public string Coordinates { get; set; }

        /// <summary>
        /// 分店数量
        /// </summary>

        public int BranchStoreCount
        {
            get;
            set;
        }

        /// <summary>
        /// 品牌数量
        /// </summary>
        public int ProductBrandCount
        {
            get;
            set;
        }

        /// <summary>
        /// 实时计算折扣
        /// </summary>

        public decimal DiscountRate
        {
            get;
            set;
        }

        /// <summary>
        /// 实时计算距离
        /// </summary>

        public int Distance { get; set; }

        /// <summary>
        /// 享受3000会员级别待遇
        /// </summary>

        public string ShopTag { get; set; }

        /// <summary>
        /// 服务列表
        /// </summary>
        public IEnumerable<IGrouping<string, ServiceForShopDetailModel>> Services
        {
            get;
            set;
        }
    }
}