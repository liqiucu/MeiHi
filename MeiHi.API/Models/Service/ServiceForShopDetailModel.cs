using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.API.Models
{
    public class ServiceForShopDetailModel
    {
        MeiHiEntities db = new MeiHiEntities();

        public long ServiceId { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName 
        {
            get
            {
                if (ServiceTypeId == 0)
                {
                    return "";
                }

                return db.ServiceType.FirstOrDefault(a => a.ServiceTypeId == this.ServiceTypeId).Title;
            }
        }
        public long ShopId { get; set; }
        public string Title { get; set; }
        public decimal CMUnitCost { get; set; }
        public decimal OriginalUnitCost { get; set; }
    }
}