using MeiHi.Model;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeiHi.Admin.Models.UserSuggests
{
    public class UserSuggestsModel
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public DateTime StartDateTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public DateTime EndDateTime { get; set; }

        public StaticPagedList<UserSuggest> UserSuggest { get; set; }
    }
}