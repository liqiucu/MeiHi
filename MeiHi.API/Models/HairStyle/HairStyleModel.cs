using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeiHi.API.ViewModels
{
    public class HairStylesModel
    {
        public List<HairStyleModel> HiarStyles { get; set; }

        public long TypeId { get; set; }

        public string TypeName { get; set; }
    }

    public class HairStyleModel
    {
        public long HairStyleId { get; set; }

        public string HairImage { get; set; }

        public string ModelImage { get; set; }
    }
}