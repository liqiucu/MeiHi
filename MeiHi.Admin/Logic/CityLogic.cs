using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using MeiHi.Admin.Models.City;

namespace MeiHi.Admin.Logic
{
    public static class CityLogic
    {
        public static CityRegionModel GetAllRegionsByCityId(int cityId)
        {
            using (var db = new MeiHiEntities())
            {
                CityRegionModel model = new CityRegionModel();

                //找到区域
                var regions = db.Region.Where(a => a.ParentRegionId == cityId && a.RootRegionId == cityId);
                model.RegionList = new List<RegionModel>();
                model.CityId = cityId;
                
                if (regions != null && regions.Count() > 0)
                {
                    
                    foreach (var itemRegion in regions)
                    {
                        RegionModel regionModel = new RegionModel();
                        regionModel.RegionId = itemRegion.RegionId;
                        regionModel.RegionName = itemRegion.Name;
                        //var streets = db.Region.Where(a => a.RootRegionId == cityId && a.ParentRegionId == itemRegion.RegionId);

                        //regionModel.StreetList = new List<StreetModel>();
                        //if (streets != null && streets.Count() > 0)
                        //{
                        //    foreach (var itemStreet in streets)
                        //    {
                        //        regionModel.StreetList.Add(new StreetModel()
                        //        {
                        //            RegionId = itemStreet.RegionId,
                        //            RegionName = itemStreet.Name
                        //        });
                        //    }
                        //}

                        model.RegionList.Add(regionModel);
                    }
                }

                return model;
            }
        }

        public static RegionModel GetAllStreetsByRegionId(int regionId)
        {
            using (var db = new MeiHiEntities())
            {
                var region = db.Region.FirstOrDefault(a => a.RegionId == regionId);

                if (region == null)
                {
                    throw new Exception("区域不存在");
                }

                RegionModel regionModel = new RegionModel();
                regionModel.RegionId = regionId;
                regionModel.RegionName = region.Name;
                regionModel.ParentRegionId = region.ParentRegionId.Value;
                var streets = db.Region.Where(a => a.RootRegionId == region.RootRegionId && a.ParentRegionId == region.RegionId);
                regionModel.StreetList = new List<StreetModel>();

                if (streets != null && streets.Count() > 0)
                {
                    foreach (var itemStreet in streets)
                    {
                        regionModel.StreetList.Add(new StreetModel()
                        {
                            RegionId = itemStreet.RegionId,
                            RegionName = itemStreet.Name
                        });
                    }
                }

                return regionModel;
            }
        }
    }
}