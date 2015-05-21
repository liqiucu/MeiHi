using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MeiHi.Model;
using MeiHi.API.ViewModels;

namespace MeiHi.API.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        [Route("Show_UserHome")]
        public object UserHome(long userId)
        {
            using (var db = new MeiHiEntities())
            {
                var user = db.User.FirstOrDefault(a => a.UserId == userId);

                if (user == null)
                {
                    return new
                    {
                        jsonStatus = 0,
                        resut = "用户不存在"
                    };
                }

                var userHomeModel = new UserHomeModel()
                {
                    Balance = 0,
                    MeiHiBookingVerifyCodeCount = db.Booking.Count(a => a.UserId == userId),
                    MeiHiUserName = user.FullName,
                    UserId = userId,
                    UserCommentCount = db.UserComments.Count(a => a.UserId == userId),
                    UserImageUrl = user.ProfilePhoto
                };

                return new
                {
                    jsonStatus = 0,
                    resut = userHomeModel
                };
            }
        }

        [HttpGet]
        [Route("Show_UserDetail")]
        public object UserDetail(long userId)
        {
            using (var db = new MeiHiEntities())
            {

            }
        }
    }
}
