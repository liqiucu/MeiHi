﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace MeiHi.Admin.Models
{
    public class ShopModel
    {
        public StaticPagedList<ShopListDetailModel> Lists { get; set; }
    }

    public class CreateShopMpdel
    {
        [Display(Name = "店铺ID")]
        public long ShopId { get; set; }
        [Display(Name = "区域")]
        public string RegionName { get; set; }
        [Display(Name = "品牌图片")]
        public List<string> ProductBrandList { get; set; }
        [Display(Name = "店面图片")]
        public List<string> ShopProductList { get; set; }

        [Display(Name = "区域ID")]
        public int RegionId { get; set; }

        [Display(Name = "区域")]
        public List<SelectListItem> RegionNameList
        {
            get;
            set;
        }

        [Display(Name = "街道ID")]
        public int StreetId { get; set; }

        [Display(Name = "街道")]
        public List<SelectListItem> StreetNameList
        {
            get;
            set;
        }

        [Display(Name = "银行卡号")]
        //[Required]
        public string BankNo { get; set; }

        [Display(Name = "银行名")]
        //[Required]
        public string BankName { get; set; }

        [Display(Name = "店名")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "坐标")]
        [Required]
        public string Coordinates { get; set; }

        [Display(Name = "详细地址")]
        [Required]
        public string DetailAddress { get; set; }

        [Display(Name = "店主电话用于登陆")]
        [RegularExpression(@"^1[3458][0-9]{9}$", ErrorMessage = "手机号格式不正确")]
        [Required]
        public string Phone { get; set; }

        [Display(Name = "支付宝账号")]
        public string AliPayAccount { get; set; }

        [Display(Name = "微信支付账号")]
        public string WinXinPayAccount { get; set; }

        [Display(Name = "联系人名字")]
        public string FullName { get; set; }

        [Display(Name = "店主联系方式(座机或者手机)")]
        [Required]
        public string Contract { get; set; }

        [Display(Name = "是否推荐")]
        [Required]
        public bool IsHot { get; set; }

        [Display(Name = "是否上线")]
        [Required]
        public bool IsOnline { get; set; }

        [Display(Name = "备注 联系人等等")]
        [Required]
        public string Comment { get; set; }

        [Display(Name = "父店ID")]
        public long ParentShopId { get; set; }

        [Display(Name = "父店名")]
        public string ParentShopName { get; set; }

        [Display(Name = "购买须知")]
        public string PurchaseNotes { get; set; }

        [Display(Name = "店铺Tag")]
        public string ShopTag { get; set; }
    }

    public class EditShopMpdel
    {
        [Display(Name = "店铺Id")]
        public long ShopId { get; set; }
        public int RegionId { get; set; }
        [Display(Name = "区域街道")]
        public List<SelectListItem> RegionNameList
        {
            get;
            set;
        }

        [Display(Name = "街道ID")]
        public int StreetId { get; set; }

        [Display(Name = "街道")]
        public List<SelectListItem> StreetNameList
        {
            get;
            set;
        }
        [Display(Name = "银行卡号")]
        //[Required]
        public string BankNo { get; set; }

        [Display(Name = "银行名")]
        //[Required]
        public string BankName { get; set; }
        [Display(Name = "店名")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "坐标")]
        [Required]
        public string Coordinates { get; set; }

        [Display(Name = "详细地址")]
        [Required]
        public string DetailAddress { get; set; }

        [Display(Name = "店主电话用于登陆")]
        [RegularExpression(@"^1[3458][0-9]{9}$", ErrorMessage = "手机号格式不正确")]
        [Required]
        public string Phone { get; set; }

        [Display(Name = "支付宝账号")]
        public string AliPayAccount { get; set; }

        [Display(Name = "微信支付账号")]
        public string WinXinPayAccount { get; set; }

        [Display(Name = "联系人名字")]
        public string FullName { get; set; }

        [Display(Name = "店主联系方式(座机或者手机)")]
        [Required]
        public string Contract { get; set; }

        [Display(Name = "是否推荐")]
        [Required]
        public bool IsHot { get; set; }

        [Display(Name = "是否上线")]
        [Required]
        public bool IsOnline { get; set; }

        [Display(Name = "备注 联系人等等")]
        [Required]
        public string Comment { get; set; }

        [Display(Name = "父店名")]
        public string ParentShopName { get; set; }

        [Display(Name = "购买须知")]
        public string PurchaseNotes { get; set; }

        [Display(Name = "店铺Tag")]
        public string ShopTag { get; set; }

        [Display(Name = "品牌")]
        public List<string> ProductBrandList { get; set; }
        [Display(Name = "门面")]
        public List<string> ShopProductList { get; set; }
    }

    public class ShopListDetailModel
    {
        public long ShopId { get; set; }
        public string DetailAddress { get; set; }
        public string RegionName { get; set; }
        public string Phone { get; set; }
        public string Contract { get; set; }
        public string ParentShopName { get; set; }
        public long? ParentShopId { get; set; }
        public int BranchShopCount { get; set; }
        public bool IsHot { get; set; }
        public bool IsOnline { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
    }
}