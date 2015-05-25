﻿using System;
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
using MeiHi.CommonDll.Helper;

namespace MeiHi.API.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// 当token为空的或者主动调用都需要验证码验证登陆
        /// </summary>
        /// <param name="mobileModel">手机号</param>
        /// <returns></returns>
        [Route("get_register_mobile_verification_code")]
        [HttpGet]
        public object GetRegisterMobileVerificationCode(string mobile)
        {
            using (var db = new MeiHiEntities())
            {
                var reg = db.User.SingleOrDefault(r => r.Mobile == mobile);
                var code = CommonHelper.GenerateRandomNumber(100000, 999999).ToString();
                if (reg == null)
                {
                    reg = new User()
                    {
                        Mobile = mobile,
                        VerifyCode = code,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                    };
                    db.User.Add(reg);
                    LuoSiMaoTextMessage.SendText(reg.Mobile, reg.VerifyCode + " 用于登陆");
                }
                else
                {
                    reg.VerifyCode = code;
                    reg.DateModified = DateTime.Now;
                    LuoSiMaoTextMessage.SendText(reg.Mobile, reg.VerifyCode + " 用于登陆");
                }
                db.SaveChanges();
                return new
                {
                    jsonStatus = 0,
                    result = "send mobile:" + mobile + " verify code: " + code + "success"
                };
            }
        }

        /// <summary>
        /// 一定是 已经入库的用户调用才会成功
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="verifyCode">验证码</param>
        /// <param name="device">ios ws andrio</param>
        /// <param name="version">手机版本</param>
        /// <param name="downFromAppId">从哪里下载的</param>
        /// <returns></returns>
        [Route("register_user")]
        [HttpGet]
        public object RegisterUser(
            string mobile,
            string verifyCode,
            string device,
            string version,
            int downFromAppId)
        {
            using (var db = new MeiHiEntities())
            {
                var reg = db.User.SingleOrDefault(r => r.Mobile == mobile && r.VerifyCode == verifyCode);

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
                string hashedToken = CommonHelper.GenerateHashWithSalt(mobile, salt);
                reg.Token = hashedToken;
                reg.Device = device;
                reg.DownloadFromApplicationId = downFromAppId;
                reg.FullName = string.IsNullOrEmpty(reg.FullName) ? "美嗨用户" + reg.UserId : reg.FullName;
                reg.Salt = salt;
                reg.Version = version;
                db.SaveChanges();

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 1, \"token\": \"" + reg.Token + "\", \"mobile\": " + reg.Mobile + "\", \"MeiHiUserId\": " + reg.UserId + "\"}", Encoding.UTF8, "application/json")
                };
            }
        }

        [Route("user_logout")]
        [HttpGet]
        public object LogOut(string mobile, string token)
        {
            using (var db = new MeiHiEntities())
            {
                var user = db.User.Where(a => a.Mobile == mobile && a.Token == token).FirstOrDefault();

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



