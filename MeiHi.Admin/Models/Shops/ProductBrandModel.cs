using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models
{
    public class ProductBrandModel
    {
        public long ProductBrandId { get; set; }
        public string ProductUrl { get; set; }
        public string ProductName { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
    }
}