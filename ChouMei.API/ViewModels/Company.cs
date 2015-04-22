using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChouMei.API.ViewModels
{
    public class UpdateCompanyInfo
    {
        [Required(ErrorMessage = "请输入手机号")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "请输入企业名称")]
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Introduction { get; set; }

    }

    public class PublishJobModel
    {
        [Required(ErrorMessage = "请输入标题")]
        public string Title { get; set; }
        [Required(ErrorMessage = "请选择需要性别")]
        public string Gender { get; set; }
        public int PeopleRequired { get; set; }
        [Required(ErrorMessage = "请输入联系人")]
        public string Contact { get; set; }
        [Required(ErrorMessage = "请输入联系方式")]
        public string ContactPhone { get; set; }
        [Required(ErrorMessage = "请输入联系地址")]
        public string Address { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateExpiry { get; set; }
        public DateTime WorkFrom { get; set; }
        public DateTime WorkEnd { get; set; }
        public string Description { get; set; }
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

    public class PublishActivityJobModel : PublishJobModel
    {
        public int PositionId { get; set; }
        public int RegionId { get; set; }
        public int WageUnitId { get; set; }
        public decimal Wage { get; set; }
        public bool IncludeDinner { get; set; }
        public bool IncludeRoom { get; set; }
        public int TermId { get; set; }
    }

    public class PublishCampusJobModel : PublishJobModel
    {
        public int CategoryId { get; set; }
        public int SchoolId { get; set; }
        public int RegionId { get; set; }
    }

    public class PublishInternJobModel : PublishJobModel
    {
        public int CategoryId { get; set; }
        public int RegionId { get; set; }
        public int WageUnitId { get; set; }
        public decimal Wage { get; set; }
        public bool IncludeDinner { get; set; }
        public bool IncludeRoom { get; set; }
    }

    public class PublishServiceJobModel : PublishJobModel
    {
        public int CategoryId { get; set; }
        public int PositionId { get; set; }
        public int RegionId { get; set; }
        public int WageUnitId { get; set; }
        public decimal Wage { get; set; }
        public bool IncludeDinner { get; set; }
        public bool IncludeRoom { get; set; }
        public int TermId { get; set; }
    }

    public class PublishTutorJobModel : PublishJobModel
    {
        public int LevelId { get; set; }
        public int PaperId { get; set; }
        public int RegionId { get; set; }
        public int WageUnitId { get; set; }
        public decimal Wage { get; set; }
        public bool IncludeDinner { get; set; }
        public bool IncludeRoom { get; set; }
        public int TermId { get; set; }
    }
}