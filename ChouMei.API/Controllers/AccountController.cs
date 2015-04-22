using ChouMei.API.Filters;
using ChouMei.API.ViewModels;
using ChouMei.Framework;
using ChouMei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace ChouMei.API.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : ApiController
    {
        private SiTuXiaoYuanEntities db = new SiTuXiaoYuanEntities();

        // POST api/values
        [Route("get_register_mobile_verification_code")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetRegisterMobileVerificationCode([FromBody]MobileModel mobileModel)
        {

            var reg = db.Registers.SingleOrDefault(r => r.Mobile == mobileModel.Mobile);
            if (reg == null)
            {
                reg = new Register()
                {
                    Mobile = mobileModel.Mobile,
                    MobileVerficationCode = Helper.GenerateRandomNumber(100000, 999999).ToString(),
                    DateExpired = DateTime.Now.AddHours(1),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    MobileVerified = null
                };
                db.Registers.Add(reg);
                LuoSiMaoTextMessage.SendText(reg.Mobile, reg.MobileVerficationCode);
            }
            else
            {
                if ((DateTime.Now - reg.DateModified).Minutes >= 1)
                {
                    reg.MobileVerficationCode = Helper.GenerateRandomNumber(100000, 999999).ToString();
                    reg.DateExpired = DateTime.Now.AddHours(1);
                    reg.DateModified = DateTime.Now;
                    reg.MobileVerified = null;
                    LuoSiMaoTextMessage.SendText(reg.Mobile, reg.MobileVerficationCode);
                }
            }
            db.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"jsonStatus\": 1, \"dateExpired\": \"" + reg.DateExpired.ToString(Constants.DateTimeFormat) + "\", \"dateModified\": \"" + reg.DateModified.ToString(Constants.DateTimeFormat) + "\", \"dateCurrent\": \"" + DateTime.Now.ToString(Constants.DateTimeFormat) + "\"}", Encoding.UTF8, "application/json")
            };
        }

        [Route("register_user")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage RegisterUser([FromBody]RegisterUserModel registerUserModel)
        {
            var reg = db.Registers.SingleOrDefault(r => r.Mobile == registerUserModel.Mobile && r.MobileVerficationCode == registerUserModel.MobileVerificationCode);
            if (reg == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"验证码错误\"}", Encoding.UTF8, "application/json")
                };
                throw new HttpResponseException(resp);
            }

            if (reg.DateExpired < DateTime.Now)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"验证码已过期，请返回重新验证\"}", Encoding.UTF8, "application/json")
                };
                throw new HttpResponseException(resp);
            }

            if (db.Users.Any(r => r.Mobile == registerUserModel.Mobile))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"该手机号已经被注册\"}", Encoding.UTF8, "application/json")
                };
                throw new HttpResponseException(resp);
            }
            else
            {
                reg.MobileVerified = true;
                reg.DateModified = DateTime.Now;

                string salt = Guid.NewGuid().ToString();
                string hashedPassword = Helper.GenerateHashWithSalt(registerUserModel.Password, salt);
                var u = new User()
                {
                    Mobile = registerUserModel.Mobile,
                    SchoolId = registerUserModel.SchoolId,
                    Password = hashedPassword,
                    Salt = salt,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                };
                db.Users.Add(u);
                db.SaveChanges();


                var newLogon = new UserLogon()
                {
                    UserId = u.UserId,
                    Token = Guid.NewGuid().ToString(),
                    TokenExpiryDate = DateTime.Now.AddMonths(1),
                    IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress,
                    DateLogon = DateTime.Now
                };
                db.UserLogons.Add(newLogon);
                db.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 1, \"token\": \"" + newLogon.Token + "\", \"userId\": " + u.UserId + ", \"tokenExpire\": \"" + newLogon.TokenExpiryDate.ToString(Constants.DateTimeFormat) + "\"}", Encoding.UTF8, "application/json")
                };
            }
        }

        [Route("register_company")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage RegisterCompany([FromBody]RegisterCompanyModel registerCompanyModel)
        {
            var reg = db.Registers.SingleOrDefault(r => r.Mobile == registerCompanyModel.Mobile && r.MobileVerficationCode == registerCompanyModel.MobileVerificationCode);
            if (reg == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"验证码错误\"}", Encoding.UTF8, "application/json")
                };
                throw new HttpResponseException(resp);
            }

            if (reg.DateExpired < DateTime.Now)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"验证码已过期，请返回重新验证\"}", Encoding.UTF8, "application/json")
                };
                throw new HttpResponseException(resp);
            }

            if (db.Companies.Any(r => r.Mobile == registerCompanyModel.Mobile))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"该手机号已经被注册\"}", Encoding.UTF8, "application/json")
                };
                throw new HttpResponseException(resp);
            }
            else
            {
                reg.MobileVerified = true;
                reg.DateModified = DateTime.Now;

                string salt = Guid.NewGuid().ToString();
                string hashedPassword = Helper.GenerateHashWithSalt(registerCompanyModel.Password, salt);
                var u = new Company()
                {
                    Mobile = registerCompanyModel.Mobile,
                    FullName = registerCompanyModel.CompanyName,
                    Password = hashedPassword,
                    Salt = salt,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                };
                db.Companies.Add(u);
                db.SaveChanges();


                var newLogon = new CompanyLogon()
                {
                    CompanyId = u.CompanyId,
                    Token = Guid.NewGuid().ToString(),
                    TokenExpiryDate = DateTime.Now.AddMonths(1),
                    IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress,
                    DateLogon = DateTime.Now
                };
                db.CompanyLogons.Add(newLogon);
                db.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 1, \"token\": \"" + newLogon.Token + "\", \"companyId\": " + u.CompanyId + ", \"tokenExpire\": \"" + newLogon.TokenExpiryDate.ToString(Constants.DateTimeFormat) + "\"}", Encoding.UTF8, "application/json")
                };
            }
        }

        [Route("get_reset_password_mobile_verification_code"), HttpPost]
        public HttpResponseMessage GetResetPasswordMobileVerificationCode([FromBody]MobileModel mobileModel)
        {
            var reset = db.ResetPasswords.SingleOrDefault(r => r.Mobile == mobileModel.Mobile);
            if (reset == null)
            {
                reset = new ResetPassword()
                {
                    Mobile = mobileModel.Mobile,
                    MobileVerficationCode = Helper.GenerateRandomNumber(100000, 999999).ToString(),
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    DateExpired = DateTime.Now.AddHours(1),
                    MobileVerified = null
                };
                db.ResetPasswords.Add(reset);
                LuoSiMaoTextMessage.SendText(reset.Mobile, reset.MobileVerficationCode);
            }
            else
            {
                if ((DateTime.Now - reset.DateModified).Minutes >= 1)
                {
                    reset.MobileVerficationCode = Helper.GenerateRandomNumber(100000, 999999).ToString();
                    reset.DateModified = DateTime.Now;
                    reset.DateExpired = DateTime.Now.AddHours(1);
                    reset.MobileVerified = null;

                    LuoSiMaoTextMessage.SendText(reset.Mobile, reset.MobileVerficationCode);
                }
            }
            db.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"jsonStatus\": 1,\"dateExpired\": \"" + reset.DateExpired.ToString(Constants.DateTimeFormat) + "\", \"dateModified\": \"" + reset.DateModified.ToString(Constants.DateTimeFormat) + "\", \"dateCurrent\": \"" + DateTime.Now.ToString(Constants.DateTimeFormat) + "\"}", Encoding.UTF8, "application/json")
            };
        }

        [Route("reset_user_password"), HttpPost]
        public HttpResponseMessage ResetUserPassword([FromBody]ResetPasswordModel resetPasswordModel)
        {
            var reg = db.ResetPasswords.SingleOrDefault(r => r.Mobile == resetPasswordModel.Mobile && r.MobileVerficationCode == resetPasswordModel.MobileVerificationCode);
            if (reg == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"验证码错误\"}", Encoding.UTF8, "application/json")
                };
                throw new HttpResponseException(resp);
            }

            if (reg.DateExpired < DateTime.Now)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"验证码已过期，请返回重新验证\"}", Encoding.UTF8, "application/json")
                };
                throw new HttpResponseException(resp);
            }

            HttpResponseMessage hrm = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var user = db.Users.SingleOrDefault(p => p.Mobile == resetPasswordModel.Mobile);
            if (user == null)
            {
                hrm.Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"该手机尚未注册！\"}", Encoding.UTF8, "application/json");
            }
            else
            {
                string salt = Guid.NewGuid().ToString();
                string hashedPassword = Helper.GenerateHashWithSalt(resetPasswordModel.Password, salt);
                user.Password = hashedPassword;
                user.Salt = salt;
                user.DateModified = DateTime.Now;
                db.SaveChanges();

                var logon = db.UserLogons.SingleOrDefault(r => r.UserId == user.UserId);
                if (logon == null)
                {
                    logon = new UserLogon()
                    {
                        UserId = user.UserId,
                        Token = Guid.NewGuid().ToString(),
                        TokenExpiryDate = DateTime.Now.AddMonths(1),
                        IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress,
                        DateLogon = DateTime.Now
                    };
                    db.UserLogons.Add(logon);
                }
                else
                {
                    logon.Token = Guid.NewGuid().ToString();
                    logon.TokenExpiryDate = DateTime.Now.AddMonths(1);
                    logon.IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                    logon.DateLogon = DateTime.Now;
                }
                db.SaveChanges();

                hrm.StatusCode = HttpStatusCode.OK;
                hrm.Content = new StringContent("{\"jsonStatus\": 1,\"token\": \"" + logon.Token + "\", \"tokenExpire\": \"" + logon.TokenExpiryDate.ToString(Constants.DateTimeFormat) + "\"}", Encoding.UTF8, "application/json");
            }
            return hrm;
        }

        [Route("reset_company_password"), HttpPost]
        public HttpResponseMessage ResetCompanyPassword([FromBody]ResetPasswordModel resetPasswordModel)
        {
            var reg = db.ResetPasswords.SingleOrDefault(r => r.Mobile == resetPasswordModel.Mobile && r.MobileVerficationCode == resetPasswordModel.MobileVerificationCode);
            if (reg == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"验证码错误\"}", Encoding.UTF8, "application/json")
                };
                throw new HttpResponseException(resp);
            }

            if (reg.DateExpired < DateTime.Now)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"验证码已过期，请返回重新验证\"}", Encoding.UTF8, "application/json")
                };
                throw new HttpResponseException(resp);
            }

            HttpResponseMessage hrm = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var company = db.Companies.SingleOrDefault(p => p.Mobile == resetPasswordModel.Mobile);
            if (company == null)
            {
                hrm.Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"该手机尚未注册！\"}", Encoding.UTF8, "application/json");
            }
            else
            {
                string salt = Guid.NewGuid().ToString();
                string hashedPassword = Helper.GenerateHashWithSalt(resetPasswordModel.Password, salt);
                company.Password = hashedPassword;
                company.Salt = salt;
                company.DateModified = DateTime.Now;
                db.SaveChanges();

                var logon = db.CompanyLogons.SingleOrDefault(r => r.CompanyId == company.CompanyId);
                if (logon == null)
                {
                    logon = new CompanyLogon()
                    {
                        CompanyId = company.CompanyId,
                        Token = Guid.NewGuid().ToString(),
                        TokenExpiryDate = DateTime.Now.AddMonths(1),
                        IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress,
                        DateLogon = DateTime.Now
                    };
                    db.CompanyLogons.Add(logon);
                }
                else
                {
                    logon.Token = Guid.NewGuid().ToString();
                    logon.TokenExpiryDate = DateTime.Now.AddMonths(1);
                    logon.IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                    logon.DateLogon = DateTime.Now;
                }
                db.SaveChanges();

                hrm.StatusCode = HttpStatusCode.OK;
                hrm.Content = new StringContent("{\"jsonStatus\": 1,\"token\": \"" + logon.Token + "\", \"tokenExpire\": \"" + logon.TokenExpiryDate.ToString(Constants.DateTimeFormat) + "\"}", Encoding.UTF8, "application/json");
            }
            return hrm;
        }

        [Route("user_login"), HttpPost]
        public HttpResponseMessage UserLogin([FromBody]LoginModel loginModel)
        {
            HttpResponseMessage hrm = new HttpResponseMessage(HttpStatusCode.BadRequest);
            if (loginModel == null)
            {
                hrm.Content = new StringContent("{\"jsonStatus\": -1,\"errorMessage\": \"参数不合法！\"}", Encoding.UTF8, "application/json");
            }
            else
            {
                var model = db.Users.SingleOrDefault(p => p.Mobile == loginModel.Mobile);
                if (model == null)
                {
                    hrm.Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"该手机尚未注册！\"}", Encoding.UTF8, "application/json");
                }
                else
                {
                    string salt = model.Salt;
                    string hashedPassword = Helper.GenerateHashWithSalt(loginModel.Password, salt);

                    // 登陆成功，是否需要插入登陆记录
                    if (model.Password != hashedPassword)
                        hrm.Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"密码错误！\"}", Encoding.UTF8, "application/json");
                    else
                    {
                        var logon = db.UserLogons.SingleOrDefault(r => r.UserId == model.UserId);
                        if (logon != null)
                        {
                            logon.Token = Guid.NewGuid().ToString();
                            logon.TokenExpiryDate = DateTime.Now.AddMonths(1);
                            logon.DateLogon = DateTime.Now;
                            logon.IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                        }
                        else
                        {
                            logon = new UserLogon()
                            {
                                DateLogon = DateTime.Now,
                                UserId = model.UserId,
                                Token = Guid.NewGuid().ToString(),
                                TokenExpiryDate = DateTime.Now.AddMonths(1),
                                IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress
                            };
                            db.UserLogons.Add(logon);
                        }
                        db.SaveChanges();

                        hrm.Content = new StringContent("{\"jsonStatus\": 1,\"token\": \"" + logon.Token + "\", \"userId\": " + logon.UserId + ", \"tokenExpire\": \"" + logon.TokenExpiryDate.ToString(Constants.DateTimeFormat) + "\"}", Encoding.UTF8, "application/json");
                        hrm.StatusCode = HttpStatusCode.OK;
                    }
                }
            }
            return hrm;
        }

        [Route("company_login"), HttpPost]
        public HttpResponseMessage CompanyLogin([FromBody]LoginModel loginModel)
        {
            HttpResponseMessage hrm = new HttpResponseMessage(HttpStatusCode.OK);
            if (loginModel == null)
            {
                hrm.Content = new StringContent("{\"jsonStatus\": -1,\"errorMessage\": \"参数不合法！\"}", Encoding.UTF8, "application/json");
            }
            else
            {
                var model = db.Companies.SingleOrDefault(p => p.Mobile == loginModel.Mobile);
                if (model == null)
                {
                    hrm.Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"该手机尚未注册！\"}", Encoding.UTF8, "application/json");
                }
                else
                {
                    string salt = model.Salt;
                    string hashedPassword = Helper.GenerateHashWithSalt(loginModel.Password, salt);

                    // 登陆成功，是否需要插入登陆记录
                    if (model.Password != hashedPassword)
                        hrm.Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"密码错误！\"}", Encoding.UTF8, "application/json");
                    else
                    {
                        var logon = db.CompanyLogons.SingleOrDefault(r => r.CompanyId == model.CompanyId);
                        if (logon != null)
                        {
                            logon.Token = Guid.NewGuid().ToString();
                            logon.TokenExpiryDate = DateTime.Now.AddMonths(1);
                            logon.DateLogon = DateTime.Now;
                            logon.IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                        }
                        else
                        {
                            logon = new CompanyLogon()
                            {
                                DateLogon = DateTime.Now,
                                CompanyId = model.CompanyId,
                                Token = Guid.NewGuid().ToString(),
                                TokenExpiryDate = DateTime.Now.AddMonths(1),
                                IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress
                            };
                            db.CompanyLogons.Add(logon);
                        }
                        db.SaveChanges();

                        hrm.Content = new StringContent("{\"jsonStatus\": 1,\"token\": \"" + logon.Token + "\", \"companyId\": " + logon.CompanyId + ", \"tokenExpire\": \"" + logon.TokenExpiryDate.ToString(Constants.DateTimeFormat) + "\"}", Encoding.UTF8, "application/json");
                        hrm.StatusCode = HttpStatusCode.OK;
                    }
                }
            }
            return hrm;
        }

        [Route("is_login"), HttpPost]
        public HttpResponseMessage IsLogin()
        {
            if (Request.Headers.Contains("Authorization"))
            {
                string token = Request.Headers.GetValues("Authorization").First();
                var login = db.UserLogons.SingleOrDefault(r => r.Token == token);
                if (login != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"jsonStatus\": 1, \"userId\": " + login.UserId + "}", Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var companyLogin = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
                    if (companyLogin != null)
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("{\"jsonStatus\": 1, \"companyId\": " + companyLogin.CompanyId + "}", Encoding.UTF8, "application/json")
                        };
                    }
                }
            }

            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"jsonStatus\": 0,\"alertMessage\": \"" + "登录不合法" + "\"}", Encoding.UTF8, "application/json")
            };
            throw new HttpResponseException(resp);
        }

        [Route("log_login"), HttpPost]
        [ApiAuthorize]
        public HttpResponseMessage LogLogin([FromBody] LogLoginModel logLoginModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
            if (login != null)
            {
                db.api_log_company_logon(login.CompanyId, logLoginModel.Device, ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress, "Logon", logLoginModel.VersionNumber);
            }
            else
            {
                var userlogin = db.UserLogons.SingleOrDefault(r => r.Token == token);
                if (userlogin != null)
                {
                    db.api_log_user_logon(userlogin.UserId, logLoginModel.Device, ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress, "Logon", logLoginModel.VersionNumber);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"jsonStatus\": 1 }", Encoding.UTF8, "application/json")
            };
        }

        [Route("log_logout"), HttpPost]
        [ApiAuthorize]
        public HttpResponseMessage LogLogout([FromBody] LogLoginModel logLoginModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
            if (login != null)
            {
                db.api_log_company_logon(login.CompanyId, logLoginModel.Device, ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress, "Logout", logLoginModel.VersionNumber);
            }
            else
            {
                var userlogin = db.UserLogons.SingleOrDefault(r => r.Token == token);
                if (userlogin != null)
                {
                    db.api_log_user_logon(userlogin.UserId, logLoginModel.Device, ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress, "Logout", logLoginModel.VersionNumber);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"jsonStatus\": 1 }", Encoding.UTF8, "application/json")
            };
        }
    }
}



