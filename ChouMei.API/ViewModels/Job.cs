using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChouMei.API.ViewModels
{
    public class ApplyModel
    {
        public long UserId { get; set; }
        public int JobTypeId { get; set; }
    }

    public class JobTypeModel
    {
        public int JobTypeId { get; set; }
    }

    public class TutorJobPapersModel
    {
        public string LevelIds { get; set; }
    }

    public class SearchModel
    {
        public bool? MonMorning { get; set; }
        public bool? MonAfternoon { get; set; }
        public bool? MonNight { get; set; }
        public bool? TueMorning { get; set; }
        public bool? TueAfternoon { get; set; }
        public bool? TueNight { get; set; }
        public bool? WedMorning { get; set; }
        public bool? WedAfternoon { get; set; }
        public bool? WedNight { get; set; }
        public bool? ThuMorning { get; set; }
        public bool? ThuAfternoon { get; set; }
        public bool? ThuNight { get; set; }
        public bool? FriMorning { get; set; }
        public bool? FriAfternoon { get; set; }
        public bool? FriNight { get; set; }
        public bool? SatMorning { get; set; }
        public bool? SatAfternoon { get; set; }
        public bool? SatNight { get; set; }
        public bool? SunMorning { get; set; }
        public bool? SunAfternoon { get; set; }
        public bool? SunNight { get; set; }
    }

    public class SearchActivityModel : SearchModel
    {
        public string PositionId { get; set; }
        public int? RegionId { get; set; }
        public int? TermId { get; set; }
        public string Gender { get; set; }
        public int OrderById { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchCampusModel : SearchModel
    {
        public string CategoryId { get; set; }
        public int? SchoolId { get; set; }
        public int? RegionId { get; set; }
        public string Gender { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchInternModel : SearchModel
    {
        public string CategoryId { get; set; }
        public int? RegionId { get; set; }
        public int? TermId { get; set; }
        public string Gender { get; set; }
        public int OrderById { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchServiceModel : SearchModel
    {
        public string CategoryId { get; set; }
        public string PositionId { get; set; }
        public int? RegionId { get; set; }
        public int? TermId { get; set; }
        public string Gender { get; set; }
        public int OrderById { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchTutorModel : SearchModel
    {
        public string LevelId { get; set; }
        public string PaperId { get; set; }
        public int? RegionId { get; set; }
        public int? TermId { get; set; }
        public string Gender { get; set; }
        public int OrderById { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchRecommendedModel
    {
        public int? RegionId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}