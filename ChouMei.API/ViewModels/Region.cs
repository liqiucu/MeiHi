using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChouMei.API.ViewModels
{
    public class RegionModel
    {
        public int RegionId { get; set; }
        public string Name { get; set; }
        public List<RegionModel> SubRegions { get; set; }
    }
}