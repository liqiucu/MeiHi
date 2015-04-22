using ChouMei.API.ViewModels;
using ChouMei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChouMei.API.Controllers
{
    [RoutePrefix("entity")]
    public class EntityController : ApiController
    {
        private SiTuXiaoYuanEntities db = new SiTuXiaoYuanEntities();

        [Route("bank"), HttpGet]
        public object Bank()
        {
            return new { jsonStatus = 1, results = db.Banks.Select(r => new { r.BankId, r.Name }) };
        }

        [Route("order_by"), HttpGet]
        public object OrderBy()
        {
            return new { jsonStatus = 1, results = db.OrderBies.Select(r => new { r.OrderById, r.Name }) };
        }

        [Route("school"), HttpGet]
        public object School()
        {
            return new { jsonStatus = 1, results = db.Schools.Select(r => new { r.SchoolId, r.Name }) };
        }

        [Route("term"), HttpGet]
        public object Term()
        {
            return new { jsonStatus = 1, results = db.Terms.Select(r => new { r.TermId, r.Name }) };
        }

        [Route("wage_unit"), HttpGet]
        public object WageUnit()
        {
            return new { jsonStatus = 1, results = db.WageUnits.Select(r => new { r.UnitId, r.Name }) };
        }

        [Route("region/{id}"), HttpGet]
        public object Region(int id)
        {
            var region = db.Regions.Single(r => r.RegionId == id);
            return new { jsonStatus = 1, regionId = region.RegionId, name = region.Name };
        }

        [Route("parent_region/{id}"), HttpGet]
        public object ParentRegion(int id)
        {
            var region = db.Regions.Single(r => r.RegionId == id);
            var parentRegion = db.Regions.SingleOrDefault(r => r.RegionId == region.ParentRegionId);
            if (parentRegion != null)
            {
                return new { jsonStatus = 1, regionId = parentRegion.RegionId, name = parentRegion.Name };
            }
            else
            {
                return new { jsonStatus = 1 };
            }
        }


        [Route("sub_regions/{id}"), HttpGet]
        public object SubRegions(int id)
        {
            return new { jsonStatus = 1, results = db.Regions.Where(r => r.ParentRegionId == id).Select(r => new { RegionId = r.RegionId, Name = r.Name }) };
        }

        [Route("root_regions"), HttpGet]
        public object RootRegions()
        {
            return new { jsonStatus = 1, results = db.Regions.Where(r => r.Indentation == 1).Select(r => new { RegionId = r.RegionId, Name = r.Name }) };
        }

        [Route("all_regions/")]
        [Route("all_regions/{id}")]
        [HttpGet]
        public object AllRegions(int? id = null)
        {
            List<RegionModel> regions = db.Regions.Where(r => r.RegionId == id || !id.HasValue).Select(r => new RegionModel { RegionId = r.RegionId, Name = r.Name }).ToList();
            GetAllSubRegions(regions);
            return new { jsonStatus = 1, results = regions };
        }
        private List<RegionModel> GetAllSubRegions(List<RegionModel> regions)
        {
            foreach (var r in regions)
            {
                var subs = db.Regions.Where(x => x.ParentRegionId == r.RegionId).Select(x => new RegionModel { RegionId = x.RegionId, Name = x.Name }).ToList();
                r.SubRegions = subs;
                if (subs.Count > 0)
                {
                    GetAllSubRegions(subs);
                }
            }
            return regions;
        }

        [Route("complain_status"), HttpGet]
        public object ComplainStatus()
        {
            return new { jsonStatus = 1, results = db.ComplainStatus.Select(r => new { r.StatusId, r.Name }) };
        }

        [Route("job_status"), HttpGet]
        public object JobStatus()
        {
            return new { jsonStatus = 1, results = db.JobStatus.Select(r => new { r.StatusId, r.Name }) };
        }

        [Route("job_type"), HttpGet]
        public object JobType()
        {
            return new { jsonStatus = 1, results = db.JobTypes.Select(r => new { r.TypeId, r.Name }) };
        }

        [Route("activity_job_position"), HttpGet]

        public object ActivityJobPosition()
        {
            return new { jsonStatus = 1, results = db.ActivityJobPositions.Select(r => new { r.PositionId, r.Name }) };
        }

        [Route("campus_job_category"), HttpGet]
        public object CampusJobCategory()
        {
            return new { jsonStatus = 1, results = db.CampusJobCategories.Select(r => new { r.CategoryId, r.Name }) };
        }

        [Route("intern_job_category"), HttpGet]
        public object InternJobCategory()
        {
            return new { jsonStatus = 1, results = db.InternJobCategories.Select(r => new { r.CategoryId, r.Name }) };
        }

        [Route("tutor_job_level"), HttpGet]
        public object TutorJobLevel()
        {
            return new { jsonStatus = 1, results = db.TutorJobLevels.Select(r => new { r.LevelId, r.Name }) };
        }

        //[Route("tutor_job_paper/{id}"), HttpGet]
        //public object TutorJobPaper(int id)
        //{
        //    return new { jsonStatus = 1, results = db.TutorJobPapers.Where(r => r.LevelId == id).Select(r => new { r.PaperId, r.Name }) };
        //}

        //[Route("tutor_job_papers"), HttpPost]
        //public object TutorJobPapers([FromBody] TutorJobPapersModel tutorJobPapersModel)
        //{
        //    return new { jsonStatus = 1, results = db.api_get_papers_by_level(tutorJobPapersModel.LevelIds) };
        //}

        [Route("service_job_category"), HttpGet]
        public object ServiceJobCategory()
        {
            return new { jsonStatus = 1, results = db.ServiceJobCategories.Select(r => new { r.CategoryId, r.Name }) };
        }

        [Route("service_job_position"), HttpGet]
        public object ServiceJobPosition()
        {
            return new { jsonStatus = 1, results = db.ServiceJobPositions.Select(r => new { r.PositionId, r.Name }) };
        }

        [Route("apply_status"), HttpGet]
        public object ApplyStatus()
        {
            return new { jsonStatus = 1, results = db.ApplyStatus.Select(r => new { r.StatusId, r.Name }) };
        }

    }
}
