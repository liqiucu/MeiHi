using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models.Advertising
{
    public class AdvertisingModel
    {
        [Display(Name="图片地址")]
        public string Url { get; set; }

        [Display(Name="标题")]
        public string Title { get; set; }

        [Display(Name = "是否有效")]
        public bool Avaliable { get; set; }

        public long AddId { get; set; }
    }
}