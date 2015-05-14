using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.API.ViewModels
{
    public class MobileModel
    {
        [Required(ErrorMessage="请输入手机号")]
        public string Mobile { get; set; }
    }

    public class RegisterUserModel
    {
        [Required(ErrorMessage = "请输入手机号")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "请输入手机验证码")]
        public string VerifyCode { get; set; }

        public string UserFullName { get; set; }

        public string Device { get; set; }
        public string Version { get; set; }
        public Nullable<int> DownloadFromApplicationId { get; set; }
        public string Token { get; set; }
        public bool IsLogin { get; set; }
    }
    public class RegisterCompanyModel
    {
        [Required(ErrorMessage = "请输入手机号")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "请输入手机验证码")]
        public string MobileVerificationCode { get; set; }

        [Required(ErrorMessage = "请输入企业名称")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }
    }
    public class LoginModel
    {
        public string Mobile { get; set; }
        public string Token { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "请输入手机号")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "请输入新密码")]
        public string Password { get; set; }
        [Required(ErrorMessage = "请输入验证码")]
        public string MobileVerificationCode { get; set; }
    }

    public class LogLoginModel
    {
        public string Device { get; set; }
        public string VersionNumber { get; set; }
    }
}