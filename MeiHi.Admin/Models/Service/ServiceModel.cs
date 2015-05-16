using MeiHi.Admin.Models.UserComments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeiHi.Admin.Models.Service
{
    public class CreateServiceModel
    {
        public long ShopId { get; set; }
        public long ServiceId { get; set; }
        public int ServiceTypeId { get; set; }
        public List<SelectListItem> ServiceTypeLists { get; set; }
        public string TitleUrl { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public decimal CMUnitCost { get; set; }
        public decimal OriginalUnitCost { get; set; }
        public string Designer { get; set; }
        // public string PurchaseNotes { get; set; }
        public bool? IfSupportRealTimeRefund { get; set; }
    }

    public class ServiceDetailModel
    {
        public long ShopId { get; set; }
        public long ServiceId { get; set; }
        public string ServiceTypeName { get; set; }
        public string TitleUrl { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public decimal CMUnitCost { get; set; }
        public decimal OriginalUnitCost { get; set; }
       // public string PurchaseNotes { get; set; }
        public string Designer { get; set; }
        public bool? IfSupportRealTimeRefund { get; set; }
    }
    //public class EditServiceModel
    //{
    //    public long ShopId { get; set; }
    //    public long ServiceId { get; set; }
    //    public int ServiceTypeId { get; set; }
    //    public List<SelectListItem> ServiceTypeLists { get; set; }
    //    public string TitleUrl { get; set; }
    //    public string Title { get; set; }
    //    public string Detail { get; set; }
    //    public decimal CMUnitCost { get; set; }
    //    public decimal OriginalUnitCost { get; set; }
    //    // public string PurchaseNotes { get; set; }
    //    public string Designer { get; set; }
    //    public bool? IfSupportRealTimeRefund { get; set; }
    //}
    public class ShowServiceListModel
    {
        public long ServiceId { get; set; }
        public string ServiceTypeName { get; set; }

        public string Title { get; set; }
        public decimal CMUnitCost { get; set; }
        public decimal OriginalUnitCost { get; set; }

        public bool? IfSupportRealTimeRefund { get; set; }
    }

    public class ShowServiceModel
    {
        public long ShopId { get; set; }

        public List<ShowServiceListModel> ShowServiceList { get; set; }
    }
}