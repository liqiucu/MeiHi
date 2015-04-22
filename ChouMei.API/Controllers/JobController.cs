using ChouMei.API.ViewModels;
using ChouMei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ChouMei.API.Controllers
{
    [RoutePrefix("job")]
    public class JobController : ApiController
    {
        private SiTuXiaoYuanEntities db = new SiTuXiaoYuanEntities();
        [Route("{id}")]
        [HttpPost]
        public object Job(long id, [FromBody]JobTypeModel jobTypeModel)
        {
            switch (jobTypeModel.JobTypeId)
            {
                case 1:
                    var r1 = db.api_get_activity_job_detail(id,1).FirstOrDefault();
                    if (r1 != null)
                        return new { jsonStatus = 1, result = r1 };
                    break;
                case 2:
                    var r2 = db.api_get_campus_job_detail(id,1).FirstOrDefault();
                    if (r2 != null)
                        return new { jsonStatus = 1, result = r2 };
                    break;
                case 3:
                    var r3 = db.api_get_intern_job_detail(id,1).FirstOrDefault();
                    if (r3 != null)
                        return new { jsonStatus = 1, result = r3 };
                    break;
                case 4:
                    var r4 = db.api_get_service_job_detail(id,1).FirstOrDefault();
                    if (r4 != null)
                        return new { jsonStatus = 1, result = r4 };
                    break;
                case 5:
                    var r5 = db.api_get_tutor_job_detail(id,1).FirstOrDefault();
                    if (r5 != null)
                        return new { jsonStatus = 1, result = r5 };
                    break;
                default:
                    break;
            }

            return new { jsonStatus = 1, result = new { } };
        }

        [HttpPost]
        [Route("search_activity")]
        public object SearchActivity([FromBody]SearchActivityModel searchActivityModel)
        {

            return new
            {
                jsonStatus = 1,
                results = db.api_job_search_activity(searchActivityModel.PositionId, searchActivityModel.RegionId, searchActivityModel.TermId,
                    searchActivityModel.Gender == "男" ? 1 : (searchActivityModel.Gender == "女" ? 0 : (int?)null), searchActivityModel.MonMorning, searchActivityModel.MonAfternoon, searchActivityModel.MonNight,
                    searchActivityModel.TueMorning, searchActivityModel.TueAfternoon, searchActivityModel.TueNight, searchActivityModel.WedMorning, searchActivityModel.WedAfternoon, searchActivityModel.WedNight,
                    searchActivityModel.ThuMorning, searchActivityModel.ThuAfternoon, searchActivityModel.ThuNight, searchActivityModel.FriMorning, searchActivityModel.FriAfternoon, searchActivityModel.FriNight,
                    searchActivityModel.SatMorning, searchActivityModel.SatAfternoon, searchActivityModel.SatNight, searchActivityModel.SunMorning, searchActivityModel.SunAfternoon, searchActivityModel.SunNight,
                    searchActivityModel.OrderById, searchActivityModel.PageIndex, searchActivityModel.PageSize)
            };
        }

        [HttpPost]
        [Route("search_campus")]
        public object SearchCampus([FromBody]SearchCampusModel searchCampusModel)
        {

            return new
            {
                jsonStatus = 1,
                results = db.api_job_search_campus(searchCampusModel.CategoryId, searchCampusModel.SchoolId, searchCampusModel.RegionId, 
                    searchCampusModel.Gender == "男" ? 1 : (searchCampusModel.Gender == "女" ? 0 : (int?)null), searchCampusModel.MonMorning, searchCampusModel.MonAfternoon, searchCampusModel.MonNight,
                    searchCampusModel.TueMorning, searchCampusModel.TueAfternoon, searchCampusModel.TueNight, searchCampusModel.WedMorning, searchCampusModel.WedAfternoon, searchCampusModel.WedNight,
                    searchCampusModel.ThuMorning, searchCampusModel.ThuAfternoon, searchCampusModel.ThuNight, searchCampusModel.FriMorning, searchCampusModel.FriAfternoon, searchCampusModel.FriNight,
                    searchCampusModel.SatMorning, searchCampusModel.SatAfternoon, searchCampusModel.SatNight, searchCampusModel.SunMorning, searchCampusModel.SunAfternoon, searchCampusModel.SunNight,
                    searchCampusModel.PageIndex, searchCampusModel.PageSize)
            };
        }

        [HttpPost]
        [Route("search_intern")]
        public object SearchIntern([FromBody]SearchInternModel searchInternModel)
        {

            return new
            {
                jsonStatus = 1,
                results = db.api_job_search_intern(searchInternModel.CategoryId, searchInternModel.RegionId, 
                    searchInternModel.Gender == "男" ? 1 : (searchInternModel.Gender == "女" ? 0 : (int?)null), searchInternModel.MonMorning, searchInternModel.MonAfternoon, searchInternModel.MonNight,
                    searchInternModel.TueMorning, searchInternModel.TueAfternoon, searchInternModel.TueNight, searchInternModel.WedMorning, searchInternModel.WedAfternoon, searchInternModel.WedNight,
                    searchInternModel.ThuMorning, searchInternModel.ThuAfternoon, searchInternModel.ThuNight, searchInternModel.FriMorning, searchInternModel.FriAfternoon, searchInternModel.FriNight,
                    searchInternModel.SatMorning, searchInternModel.SatAfternoon, searchInternModel.SatNight, searchInternModel.SunMorning, searchInternModel.SunAfternoon, searchInternModel.SunNight,
                    searchInternModel.OrderById, searchInternModel.PageIndex, searchInternModel.PageSize)
            };
        }

        [HttpPost]
        [Route("search_service")]
        public object SearchService([FromBody]SearchServiceModel searchServiceModel)
        {

            return new
            {
                jsonStatus = 1,
                results = db.api_job_search_service(searchServiceModel.CategoryId, searchServiceModel.PositionId, searchServiceModel.RegionId, searchServiceModel.TermId,
                    searchServiceModel.Gender == "男" ? 1 : (searchServiceModel.Gender == "女" ? 0 : (int?)null), searchServiceModel.MonMorning, searchServiceModel.MonAfternoon, searchServiceModel.MonNight,
                    searchServiceModel.TueMorning, searchServiceModel.TueAfternoon, searchServiceModel.TueNight, searchServiceModel.WedMorning, searchServiceModel.WedAfternoon, searchServiceModel.WedNight,
                    searchServiceModel.ThuMorning, searchServiceModel.ThuAfternoon, searchServiceModel.ThuNight, searchServiceModel.FriMorning, searchServiceModel.FriAfternoon, searchServiceModel.FriNight,
                    searchServiceModel.SatMorning, searchServiceModel.SatAfternoon, searchServiceModel.SatNight, searchServiceModel.SunMorning, searchServiceModel.SunAfternoon, searchServiceModel.SunNight,
                    searchServiceModel.OrderById, searchServiceModel.PageIndex, searchServiceModel.PageSize)
            };
        }

        [HttpPost]
        [Route("search_tutor")]
        public object SearchTutor([FromBody]SearchTutorModel searchTutorModel)
        {

            return new
            {
                jsonStatus = 1,
                results = db.api_job_search_tutor(searchTutorModel.LevelId, searchTutorModel.PaperId, searchTutorModel.RegionId, searchTutorModel.TermId,
                    searchTutorModel.Gender == "男" ? 1 : (searchTutorModel.Gender == "女" ? 0 : (int?)null), searchTutorModel.MonMorning, searchTutorModel.MonAfternoon, searchTutorModel.MonNight,
                    searchTutorModel.TueMorning, searchTutorModel.TueAfternoon, searchTutorModel.TueNight, searchTutorModel.WedMorning, searchTutorModel.WedAfternoon, searchTutorModel.WedNight,
                    searchTutorModel.ThuMorning, searchTutorModel.ThuAfternoon, searchTutorModel.ThuNight, searchTutorModel.FriMorning, searchTutorModel.FriAfternoon, searchTutorModel.FriNight,
                    searchTutorModel.SatMorning, searchTutorModel.SatAfternoon, searchTutorModel.SatNight, searchTutorModel.SunMorning, searchTutorModel.SunAfternoon, searchTutorModel.SunNight,
                    searchTutorModel.OrderById, searchTutorModel.PageIndex, searchTutorModel.PageSize)
            };
        }

        [HttpPost]
        [Route("search_recommended")]
        public object SearchRecommended([FromBody]SearchRecommendedModel searchRecommendedModel)
        {

            return new
            {
                jsonStatus = 1,
                results = db.api_job_search_recommended(searchRecommendedModel.RegionId, searchRecommendedModel.PageIndex, searchRecommendedModel.PageSize)
            };
        }
    }
}
