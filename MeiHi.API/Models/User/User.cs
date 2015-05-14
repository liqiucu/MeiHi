using MeiHi.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.API.ViewModels
{
    //public class UserInfoModel
    //{
    //    public long UserId { get; set; }
    //    public string Mobile { get; set; }
    //    public int SchoolId { get; set; }
    //    public string SchoolName { get; set; }
    //    public string FullName { get; set; }
    //    public string DateOfBirth { get; set; }
    //    public string Gender { get; set; }
    //    public string Tall { get; set; }
    //    public string RegionId { get; set; }
    //    public string RegionName { get; set; }
    //    public string ProfilePhoto { get; set; }
    //    public string QQ { get; set; }
    //    public string Email { get; set; }
    //    public string Major { get; set; }
    //    public string WorkExperience { get; set; }
    //    public decimal Balance { get; set; }
    //    public string AccountNo { get; set; }
    //    public string AccountName { get; set; }
    //    public string BankId { get; set; }
    //    public string BankName { get; set; }
    //    public List<api_get_user_job_applied_Result> CVs { get; set; }
        
    //}

    public class UpdateUserInfo
    {
        [Required(ErrorMessage = "请输入手机号")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "请选择学校")]
        public int? SchoolId { get; set; }
        [Required(ErrorMessage = "请输入姓名")]
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int? Tall { get; set; }
        public int? RegionId { get; set; }
        public string QQ { get; set; }
        public string Email { get; set; }
        public string Major { get; set; }
        public string WorkExperience { get; set; }
        public bool RequireCVs { get; set; }

    }

    public class RequireCVModel
    {
        public bool RequireCVs { get; set; }

    }

    public class PagingModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

    }

    public class CashOutModel
    {
        [Required(ErrorMessage = "请输入体现金额")]
        public decimal? Amount { get; set; }

    }

    public class CreateBankAccountModel
    {
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public int BankId { get; set; }
    }

    public class JobApplyModel
    {
        public int JobTypeId  { get; set; }
    }

    public class JobComplainModel
    {
        public int JobTypeId { get; set; }
        public int StatusId { get; set; }
        public string Description { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "请输入旧密码")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "请输入新密码")]
        public string NewPassword { get; set; }

    }
}