using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using MeiHi.Model;
using System.Web.Mvc;
using PagedList;

namespace MeiHi.Admin.ViewModels
{
    public class AccountManageModel
    {
        public StaticPagedList<AdminModel> Admins { get; set; }
    }

    public class RoleModel
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public List<string> PermissionNames { get; set; }
    }

    public class PermissionModel
    {
        public int PermissionId { get; set; }

        public string PermissionName { get; set; }
    }

    public class AdminModel
    {
        [Display(Name = "用户Id")]
        public int AdminId { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }
        [Display(Name = "密码")]
        public string Password { get; set; }
        [Display(Name = "移动电话")]
        public string Mobile { get; set; }
        [Display(Name = "是否有效")]
        public bool Avaliable { get; set; }
        [Display(Name = "角色")]
        public List<string> RoleNmes { get; set; }
        [Display(Name = "权限")]
        public List<string> PermissionNames { get; set; }
    }

    public class CreateAdminModel
    {
        [Required(ErrorMessage = "用户名必须填写")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "移动电话必须填写")]
        [Display(Name = "移动电话")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "密码必须填写")]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "是否有效")]
        public bool Avaliable { get; set; }

        [Display(Name = "角色列表")]
        public List<SelectListItem> Roles { get; set; }

        [Display(Name = "权限列表")]
        public List<SelectListItem> Permissions { get; set; }
    }

    public class EditAdminModel
    {
        public int AdminId { get; set; }

        [Required(ErrorMessage = "用户名必须填写")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "移动电话必须填写")]
        [Display(Name = "移动电话")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "密码必须填写")]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "是否有效")]
        public bool Avaliable { get; set; }

        [Display(Name = "角色列表")]
        public List<SelectListItem> Roles { get; set; }

        [Display(Name = "权限列表")]
        public List<SelectListItem> Permissions { get; set; }
    }
}
