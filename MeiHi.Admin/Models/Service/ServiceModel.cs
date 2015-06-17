using MeiHi.Admin.Models.UserComments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeiHi.Admin.Models.Service
{
    public class CreateServiceModel
    {
        [Display(Name = "店铺ID")]
        public long ShopId { get; set; }
        [Display(Name = "服务ID")]
        public long ServiceId { get; set; }
        [Display(Name = "服务类型ID")]
        public int ServiceTypeId { get; set; }
        [Display(Name = "服务类型")]
        public List<SelectListItem> ServiceTypeLists { get; set; }
        [Display(Name = "图片")]
        public string TitleUrl { get; set; }
        [Display(Name = "标题")]
        public string Title { get; set; }
        [Display(Name = "详情")]
        public string Detail { get; set; }
        [Display(Name = "美嗨价")]
        public decimal CMUnitCost { get; set; }
        [Display(Name = "原价")]
        public decimal OriginalUnitCost { get; set; }
        [Display(Name = "设计师")]
        public string Designer { get; set; }
        // public string PurchaseNotes { get; set; }
        [Display(Name = "是否支持实时退款")]
        public bool? IfSupportRealTimeRefund { get; set; }
    }

    public class ServiceDetailModel
    {
        [Display(Name = "店铺ID")]
        public long ShopId { get; set; }
        [Display(Name = "服务ID")]
        public long ServiceId { get; set; }
        [Display(Name = "服务类型")]
        public string ServiceTypeName { get; set; }
        [Display(Name = "图片")]
        public string TitleUrl { get; set; }
        [Display(Name = "标题")]
        public string Title { get; set; }
        [Display(Name = "详情")]
        public string Detail { get; set; }
        [Display(Name = "美嗨价")]
        public decimal CMUnitCost { get; set; }
        [Display(Name = "原价")]
        public decimal OriginalUnitCost { get; set; }
       // public string PurchaseNotes { get; set; }
        [Display(Name="设计师")]
        public string Designer { get; set; }
        [Display(Name = "是否支持实时退款")]
        public bool? IfSupportRealTimeRefund { get; set; }
    }

    public class ShowServiceListModel
    {
        [Display(Name = "服务ID")]
        public long ServiceId { get; set; }
        [Display(Name = "服务类型")]
        public string ServiceTypeName { get; set; }
        [Display(Name = "标题")]
        public string Title { get; set; }
        [Display(Name = "美嗨价")]
        public decimal CMUnitCost { get; set; }
        [Display(Name = "原价")]
        public decimal OriginalUnitCost { get; set; }
        [Display(Name = "是否支持实时退款")]
        public bool? IfSupportRealTimeRefund { get; set; }
    }

    public class ShowServiceModel
    {
        [Display(Name = "店铺ID")]
        public long ShopId { get; set; }

        [Display(Name = "店铺名")]
        public string ShopName { get; set; }

        public List<ShowServiceListModel> ShowServiceList { get; set; }
    }

    public class ServiceTypeModel
    {
        [Display(Name="服务类型ID")]
        public int ServiceTypeId { get; set; }
        [Display(Name = "服务类型名称")]
        public string ServiceTypeName { get; set; }
    }
}