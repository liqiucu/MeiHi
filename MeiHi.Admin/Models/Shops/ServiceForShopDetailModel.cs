using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models
{
    public class ServiceForShopDetailModel
    {
        MeiHiEntities db = new MeiHiEntities();

        public long ServiceId { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName
        {
            get;set;
        }
        public long ShopId { get; set; }
        public string Title { get; set; }
        public decimal CMUnitCost { get; set; }
        public decimal OriginalUnitCost { get; set; }
    }
}