using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using MeiHi.Model;
using MeiHi.API.Helper;
using MeiHi.API.ViewModels;

namespace MeiHi.API.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// 当token为空的或者主动调用都需要验证码验证登陆
        /// </summary>
        /// <param name="mobileModel"></param>
        /// <returns></returns>
        [Route("get_register_mobile_verification_code")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetRegisterMobileVerificationCode([FromBody]MobileModel mobileModel)
        {
            using (var db = new MeiHiEntities())
            {
                var reg = db.User.SingleOrDefault(r => r.Mobile == mobileModel.Mobile);
                if (reg == null)
                {
                    reg = new User()
                    {
                        Mobile = mobileModel.Mobile,
                        VerifyCode = Helper.Helper.GenerateRandomNumber(100000, 999999).ToString(),
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                    };
                    db.User.Add(reg);
                    LuoSiMaoTextMessage.SendText(reg.Mobile, reg.VerifyCode);
                }
                else
                {
                    reg.VerifyCode = Helper.Helper.GenerateRandomNumber(100000, 999999).ToString();
                    reg.DateModified = DateTime.Now;
                    LuoSiMaoTextMessage.SendText(reg.Mobile, reg.VerifyCode);
                }
                db.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 1, \"message\": send verify code success}", Encoding.UTF8, "application/json")
                };
            }
        }

        /// <summary>
        /// 一定是 已经入库的用户调用才会成功
        /// </summary>
        /// <param name="registerUserModel"></param>
        /// <returns></returns>
        [Route("register_user")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage RegisterUser([FromBody]RegisterUserModel registerUserModel)
        {
            using (var db = new MeiHiEntities())
            {
                var reg = db.User.SingleOrDefault(r => r.Mobile == registerUserModel.Mobile && r.VerifyCode == registerUserModel.VerifyCode);

                if (reg == null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"验证码错误\"}", Encoding.UTF8, "application/json")
                    };
                    throw new HttpResponseException(resp);
                }

                if (reg.DateModified.AddMinutes(1) < DateTime.Now)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"验证码已过期，请返回重新验证\"}", Encoding.UTF8, "application/json")
                    };
                    throw new HttpResponseException(resp);
                }

                reg.IsLogin = true;
                reg.DateModified = DateTime.Now;

                string salt = Guid.NewGuid().ToString();
                string hashedToken = Helper.Helper.GenerateHashWithSalt(registerUserModel.Mobile, salt);
                reg.Token = hashedToken;
                reg.Device = registerUserModel.Device;
                reg.DownloadFromApplicationId = registerUserModel.DownloadFromApplicationId;
                reg.FullName = string.IsNullOrEmpty(reg.FullName) ? "美嗨用户" + reg.UserId : reg.FullName;
                reg.Salt = salt;
                reg.Version = registerUserModel.Version;
                db.SaveChanges();

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 1, \"token\": \"" + reg.Token + "\", \"mobile\": " + reg.Mobile + "\", \"MeiHiUserId\": " + reg.UserId + "\"}", Encoding.UTF8, "application/json")
                };
            }
        }

        [Route("user_logout"), HttpPost]
        public object LogOut([FromBody]LoginModel loginModel)
        {
            using (var db = new MeiHiEntities())
            {
                var user = db.User.Where(a => a.Mobile == loginModel.Mobile && a.Token == loginModel.Token).FirstOrDefault();

                if (user != null)
                {
                    user.IsLogin = false;
                    user.Salt = "";
                    user.Token = "";
                    user.VerifyCode = "";
                    user.DateModified = DateTime.Now;
                    db.SaveChanges();

                    return new
                    {
                        jsonStatus = 1,
                        resut = "登出成功"
                    };
                }

                return new
                {
                    jsonStatus = 0,
                    resut = "登出失败,用户不存在！"
                };
            }
        }
    }
}



