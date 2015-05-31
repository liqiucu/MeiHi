using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models.Shoper
{
    public class ShopersModel
    {
        public StaticPagedList<ShoperModel> Shopers { get; set; }
    } 

    public class ShoperModel
    {
        public long ShoperId { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "店铺备注")]
        public string ShopComment { get; set; }

        [Display(Name = "支付宝账号")]
        public string AliPayAccount { get; set; }

        [Display(Name = "微信支付账号")]
        public string WinXinPayAccount { get; set; }

        [Display(Name = "联系人名字")]
        public string FullName { get; set; }

        [Display(Name = "店铺名")]
        public string ShopName { get; set; }

        [Display(Name = "店铺ID")]
        public long ShopId { get; set; }

        [Display(Name = "未结算单数")]
        public int UnBillingedCount { get; set; }

        [Display(Name = "未结算总金额")]
        public decimal SumUnBillinged{ get; set; }
    }

    public class PayAllUnBillingBookingsModel
    {
        public long ShopId { get; set; }

        public decimal Cost{ get; set; }

        public string AliAccount { get; set; }

        public string WeiXinAccount { get; set; }
    }
}