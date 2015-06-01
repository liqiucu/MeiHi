using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.Shop.ViewModels
{
    public class ShoperModelPassword
    {
        public long ShopId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        [StringLength(8,MinimumLength=6)]
        public string Password1 { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "重新输入新密码")]
        [StringLength(8, MinimumLength = 6)]
        public string Password2 { get; set; }
    }
}