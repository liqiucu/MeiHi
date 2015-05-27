using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.API.ViewModels
{
    public class UserAccountInfoModel
    {
        public long UserId { get; set; }

        public decimal Balance { get; set; }

        public string Mobile { get; set; }
    }
}