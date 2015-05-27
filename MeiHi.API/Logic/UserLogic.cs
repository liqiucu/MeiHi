using MeiHi.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using MeiHi.API.Helper;

namespace MeiHi.API.Logic
{
    public static class UserLogic
    {
        public static string GetUserNameById(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var user = db.User.Where(a => a.UserId == userId).FirstOrDefault();

                if (user != null)
                {
                    return user.FullName;
                }

                return "";
            }
        }

        private static readonly object CacheLockObjectGetUserByUserId = new object();

        public static User GetUserByUserId(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var user = HttpRuntime.Cache.Get("GetUserByUserId" + userId) as User;

                if (user == null)
                {
                    lock (CacheLockObjectGetUserByUserId)
                    {
                        user = db.User.FirstOrDefault(a => a.UserId == userId);
                        HttpRuntime.Cache.Insert("GetUserByUserId" + userId, user, null,
                           DateTime.Now.AddSeconds(300), TimeSpan.Zero);
                    }
                }

                return user;
            }
        }

        private static readonly object CacheLockObjectGetUserCommentsByUserId = new object();

        public static List<UserComments> GetUserCommentsByUserId(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var userComments = HttpRuntime.Cache.Get("GetUserCommentsByUserId" + userId) as List<UserComments>;

                if (userComments == null)
                {
                    lock (CacheLockObjectGetUserCommentsByUserId)
                    {
                        userComments = db.UserComments.Where(a => a.ShopId == userId && a.Display == true).ToList();
                        HttpRuntime.Cache.Insert("GetUserCommentsByUserId" + userId, userComments, null,
                           DateTime.Now.AddSeconds(600), TimeSpan.Zero);
                    }
                }

                return userComments;
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