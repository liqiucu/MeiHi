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
        public List<RoleModel> ListRoles { get; set; }
        public StaticPagedList<AdminModel> ListAdmins { get; set; }

        public List<SelectListItem> DropListRoles
        {
            get
            {
                List<SelectListItem> Lists = new List<SelectListItem>();

                ListRoles.ForEach((item) =>
                {
                    Lists.Add(new SelectListItem()
                    {
                        Text = item.RoleName,
                        Value = item.RoleId.ToString()
                    });
                });

                return Lists;
            }            
        }

        public string RoleName { get; set; }
    }

    public class RoleModel
    {
        [Display(Name = "Role Id", Order = 1)]
        public int RoleId { get; set; }

        [Display(Name = "Role Name", Order = 2)]
        public string RoleName { get; set; }
    }

    public class AdminModel
    {
        [Display(Name = "User Id", Order = 0)]
        public int UserId { get; set; }
        [Display(Name = "User Name", Order = 2)]
        public string UserName { get; set; }

        [Display(Name = "Email", Order = 3)]
        public string Email { get; set; }

        public string Salt { get; set; }

        public string Role { get; set; }

        public string Password { get; set; }
    }
}
