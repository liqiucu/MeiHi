using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models.HairStyle
{
    public class HairStyleModel
    {
        public long HairStyleId { get; set; }

        public string HairStyleTypeName { get; set; }

        public long HairStyleTypeId { get; set; }

        public string ModelUrl { get; set; }

        public string StyleUrl { get; set; }
    }

    public class HairStylesModel
    {
        public string HairStyleTypeName { get; set; }

        public long HairStyleTypeId { get; set; }

        public List<HairStyleModel> Styles { get; set; }
    }

    public class HairStyleTypeModel
    {
        [Display(Name = "类型")]
        public string HairStyleTypeName { get; set; }

        [Display(Name = "类型ID")]
        public long HairStyleTypeId { get; set; }
    }
}