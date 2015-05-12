using MeiHi.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using MeiHi.API.Helper;
using MeiHi.API.Models.UserComments;

namespace MeiHi.API.Logic
{
    public static class UserLogic
    {
        public static string GetUserNameById(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var user=db.User.Where(a => a.UserId == userId).FirstOrDefault();

                if (user != null)
                {
                    return user.FullName;
                }

                return "";
            }
        }

        //public static List<string> GetSharedImagesById(long id)
        //{
        //    using (var db = new MeiHiEntities())
        //    {
        //        var user = db.Shop.UserComments.Where(a => a.UserId == userId).FirstOrDefault();

        //        if (user != null)
        //        {
        //            return user.FullName;
        //        }

        //        return "";
        //    }
        //}
    }
}