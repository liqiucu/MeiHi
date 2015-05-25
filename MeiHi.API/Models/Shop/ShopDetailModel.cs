using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;

namespace MeiHi.API.ViewModels
{
    public class ShopDetailModel
    {
        MeiHiEntities db = new MeiHiEntities();
        public long ShopId { get; set; }
        public string DetailAddress { get; set; }

        /// <summary>
        /// 店铺title
        /// </summary>
        public string ShopTitle { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 坐标
        /// </summary>
        public string Coordinates { get; set; }

        /// <summary>
        /// 享受3000会员级别待遇
        /// </summary>
        public string ShopTag { get; set; }
        public string ProductBrandId { get; set; }

        /// <summary>
        /// 品牌数量
        /// </summary>
        public int ProductBrandCount
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
        /// 服务列表
        /// </summary>
        public IEnumerable<IGrouping<string, ServiceForShopDetailModel>> Services
        {
            get
            {
                if (this.ShopId == 0)
                {
                    return null;
                }

                var services = from a in db.Service
                               where a.ShopId == this.ShopId
                               select a;

                var result = new List<ServiceForShopDetailModel>();

                foreach (var item in services)
                {
                    var temp = new ServiceForShopDetailModel
                    {
                        CMUnitCost = item.CMUnitCost,
                        OriginalUnitCost = item.OriginalUnitCost,
                        ServiceId = item.ServiceId,
                        Title = item.Title,
                        ShopId = item.ShopId,
                        ServiceTypeId = item.ServiceTypeId
                    };
                    result.Add(temp);
                }

                var serviceGroups = result.GroupBy(a => a.ServiceTypeName);
                return serviceGroups;
            }
        }
    }
}