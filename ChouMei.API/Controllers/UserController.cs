using ChouMei.API.ViewModels;
using ChouMei.Framework;
using ChouMei.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ChouMei.API.Controllers
{
    [RoutePrefix("user")]
    public class UserController : ApiController
    {
        private SiTuXiaoYuanEntities db = new SiTuXiaoYuanEntities();
        [ApiAuthorize]
        [HttpPost]
        public object Get([FromBody]RequireCVModel requireCVModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.UserLogons.SingleOrDefault(r => r.Token == token);

            var info = db.api_get_user_info(login.UserId).First();
            return new
            {
                jsonStatus = 1,
                userId = info.UserId,
                mobile = info.Mobile,
                schoolId = info.SchoolId,
                schoolName = info.SchoolName,
                fullName = info.FullName ?? "",
                dateOfBirth = info.DateofBirth ?? "",
                gender = info.Gender,
                tall = info.Tall.HasValue ? info.Tall.ToString() : "",
                regionId = info.RegionId.HasValue ? info.RegionId.ToString() : "",
                regionName = info.RegionName ?? "",
                profilePhoto = info.ProfilePhoto ?? "",
                qq = info.QQ ?? "",
                email = info.Email ?? "",
                major = info.Major ?? "",
                workExperience = info.WorkExperience ?? "",
                balance = info.Balance,
                accountName = info.AccountName ?? "",
                accountNo = info.AccountNo ?? "",
                //bankId = info.BankId.HasValue ? info.BankId.ToString() : "",
                bankName = info.BankName ?? "",
                age = info.Age,
                cvs = requireCVModel.RequireCVs ? db.api_get_user_job_applied(info.UserId, 1, 2).ToList() : new List<api_get_user_job_applied_Result>()
            };
        }

        [Route("{id}")]
        [HttpPost]
        public object GetUser(long id, [FromBody]RequireCVModel requireCVModel)
        {
            var info = db.api_get_user_info(id).FirstOrDefault();
            if (info != null)
            {
                return new
                {
                    jsonStatus = 1,
                    userId = info.UserId,
                    mobile = info.Mobile,
                    schoolId = info.SchoolId,
                    schoolName = info.SchoolName,
                    fullName = info.FullName ?? "",
                    dateOfBirth = info.DateofBirth ?? "",
                    gender = info.Gender,
                    tall = info.Tall.HasValue ? info.Tall.ToString() : "",
                    regionId = info.RegionId.HasValue ? info.RegionId.ToString() : "",
                    regionName = info.RegionName ?? "",
                    profilePhoto = info.ProfilePhoto ?? "",
                    qq = info.QQ ?? "",
                    email = info.Email ?? "",
                    major = info.Major ?? "",
                    workExperience = info.WorkExperience ?? "",
                    age = info.Age,
                    cvs = requireCVModel.RequireCVs ? db.api_get_user_job_applied(info.UserId, 1, 2).ToList() : new List<api_get_user_job_applied_Result>()
                };
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 0, \"alertMessage\": \"该用户不存在\"}", Encoding.UTF8, "application/json")
                };
            }
        }

        [Route("more_cvs/{id}")]
        [HttpPost]
        public object MoreCvs(long id, [FromBody]PagingModel pagingModel)
        {
            return new { jsonStatus = 1, results = db.api_get_user_job_applied(id, pagingModel.PageIndex, pagingModel.PageSize) };
        }

        [ApiAuthorize]
        [Route("apply")]
        [HttpPost]
        public object Apply([FromBody]PagingModel pagingModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.UserLogons.SingleOrDefault(r => r.Token == token);
            return new { jsonStatus = 1, results = db.api_get_user_job_apply(login.UserId, pagingModel.PageIndex, pagingModel.PageSize) };
        }

        [ApiAuthorize]
        [Route("update")]
        [HttpPost]
        public object Update([FromBody]UpdateUserInfo updateUserInfo)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.UserLogons.SingleOrDefault(r => r.Token == token);


            var info = db.api_update_user_info(login.UserId, updateUserInfo.Mobile, updateUserInfo.SchoolId, updateUserInfo.FullName, updateUserInfo.DateOfBirth,
                updateUserInfo.Gender == "男" ? 1 : (updateUserInfo.Gender == "女" ? 0 : (int?)null), updateUserInfo.Tall, updateUserInfo.RegionId,
                updateUserInfo.QQ, updateUserInfo.Email, updateUserInfo.Major, updateUserInfo.WorkExperience).First();

            return new
            {
                jsonStatus = 1,
                userId = info.UserId,
                mobile = info.Mobile,
                schoolId = info.SchoolId,
                schoolName = info.SchoolName,
                fullName = info.FullName ?? "",
                dateOfBirth = info.DateofBirth ?? "",
                gender = info.Gender,
                tall = info.Tall.HasValue ? info.Tall.ToString() : "",
                regionId = info.RegionId.HasValue ? info.RegionId.ToString() : "",
                regionName = info.RegionName ?? "",
                profilePhoto = info.ProfilePhoto ?? "",
                qq = info.QQ ?? "",
                email = info.Email ?? "",
                major = info.Major ?? "",
                workExperience = info.WorkExperience ?? "",
                balance = info.Balance,
                accountName = info.AccountName,
                accountNo = info.AccountNo,
                //bankId = info.BankId.HasValue ? info.BankId.ToString() : "",
                bankName = info.BankName,
                age = info.Age,
                cvs = updateUserInfo.RequireCVs ? db.api_get_user_job_applied(info.UserId, 1, 2).ToList() : new List<api_get_user_job_applied_Result>()
            };
        }

        [ApiAuthorize]
        [Route("upload_profile")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadProfile()
        {
            try
            {
                // 设置上传目录
                string tempPath = "/upload/temp/";
                var provider = new MultipartFormDataStreamProvider(HttpContext.Current.Server.MapPath(tempPath));

                // 接收数据，并保存文件
                var bodyparts = await Request.Content.ReadAsMultipartAsync(provider);

                if (bodyparts.FileData.Count == 0)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"jsonStatus\": -1, \"errorMessage\": \"提交参数不合法\"}", Encoding.UTF8, "application/json")
                    };
                    throw new HttpResponseException(resp);
                }

                string token = Request.Headers.GetValues("Authorization").First();
                var login = db.UserLogons.Single(r => r.Token == token);
                var user = db.Users.Single(r => r.UserId == login.UserId);

                // 文件在服务端的保存地址，需要的话自行 rename 或 move
                string extension = ".jpg";
                string newPath = "/upload/user/" + login.UserId + "/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";

                int i = 0;
                foreach (var img in bodyparts.FileData)
                {
                    string fileName = Guid.NewGuid().ToString();
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(newPath)))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(newPath));
                    }
                    var filePhysicalPath = HttpContext.Current.Server.MapPath(newPath + fileName + extension);
                    File.Move(img.LocalFileName, filePhysicalPath);
                    Image image = Image.FromFile(filePhysicalPath);
                    using (var resized = ImageHelper.ResizeImage(image, 100, 100))
                    {
                        //save the resized image as a jpeg with a quality of 90
                        ImageHelper.SaveJpeg(HttpContext.Current.Server.MapPath(newPath + fileName + "_100_100" + extension), resized, 100);
                    }
                    image.Dispose();
                    if (i == 0)
                    {
                        user.ProfilePhoto = newPath + fileName + extension;
                    }
                    i++;
                }
                user.DateModified = DateTime.Now;
                db.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 1, \"profilePhoto\": \"" + user.ProfilePhoto + "\" }", Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": -1, \"errorMessage\": \"" + ex.Message + "\"}", Encoding.UTF8, "application/json")
                };
            }

        }

        [ApiAuthorize]
        [Route("create_bank_account")]
        [HttpPost]
        public HttpResponseMessage CreateBankAccount([FromBody]CreateBankAccountModel createBankAccountModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.UserLogons.SingleOrDefault(r => r.Token == token);

            if (createBankAccountModel.BankId > 0)
                db.api_create_user_bank_account(login.UserId, createBankAccountModel.AccountName, createBankAccountModel.AccountNo, createBankAccountModel.BankId);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"jsonStatus\": 1}", Encoding.UTF8, "application/json")
            };
        }

        [ApiAuthorize]
        [Route("cash_out")]
        [HttpPost]
        public HttpResponseMessage CashOut([FromBody]CashOutModel cashOutModel)
        {
            //string token = Request.Headers.GetValues("Authorization").First();
            //var login = db.UserLogons.SingleOrDefault(r => r.Token == token);


            //db.api_user_cash_out(login.UserId, cashOutModel.Amount);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"jsonStatus\": 1, \"workingdays\": \"1\"}", Encoding.UTF8, "application/json")
            };
        }

        [ApiAuthorize]
        [Route("job_apply/{id}")]
        [HttpPost]
        public object JobApply(long id, [FromBody]JobApplyModel jobApplyModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.UserLogons.SingleOrDefault(r => r.Token == token);
            var apply= db.api_user_job_apply(login.UserId, id, jobApplyModel.JobTypeId).First();
            return new {
                jsonStatus = 1, 
                alertType = apply.AlertType,
                alertMessage = apply.Message
            };
        }

        [ApiAuthorize]
        [Route("job_complain/{id}")]
        [HttpPost]
        public HttpResponseMessage JobComplain(long id, [FromBody]JobComplainModel jobComplainModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.UserLogons.SingleOrDefault(r => r.Token == token);
            db.api_user_job_complain(login.UserId, id, jobComplainModel.StatusId, jobComplainModel.JobTypeId, jobComplainModel.Description);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"jsonStatus\": 1}", Encoding.UTF8, "application/json")
            };
        }

        [ApiAuthorize]
        [Route("cancel_apply/{id}")]
        [HttpPost]
        public HttpResponseMessage CancelApply(long id, [FromBody]JobTypeModel jobTypeModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.UserLogons.SingleOrDefault(r => r.Token == token);
            db.api_user_cancel_apply(login.UserId, id, jobTypeModel.JobTypeId);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"jsonStatus\": 1}", Encoding.UTF8, "application/json")
            };
        }

        [ApiAuthorize]
        [Route("change_password")]
        [HttpPost]
        public HttpResponseMessage ChangePassword([FromBody]ChangePasswordModel changePasswordModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.UserLogons.Single(r => r.Token == token);
            var user = db.Users.Single(r => r.UserId == login.UserId);
            string salt = user.Salt;
            string hashedPassword = Helper.GenerateHashWithSalt(changePasswordModel.OldPassword, salt);
            if (hashedPassword == user.Password)
            {
                login.Token = Guid.NewGuid().ToString();
                login.TokenExpiryDate = DateTime.Now.AddMonths(1);
                login.IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                login.DateLogon = DateTime.Now;
                db.SaveChanges();

                salt = Guid.NewGuid().ToString();
                hashedPassword = Helper.GenerateHashWithSalt(changePasswordModel.NewPassword, salt);
                user.Salt = salt;
                user.Password = hashedPassword;
                user.DateModified = DateTime.Now;
                db.SaveChanges();

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 1, \"token\": \"" + login.Token + "\", \"tokenExpire\": \"" + login.TokenExpiryDate.ToString(Constants.DateTimeFormat) + "\"}", Encoding.UTF8, "application/json")
                };
            }
            else


            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 0, \"alertMessage\": \"旧密码错误，请重新输入。\"}", Encoding.UTF8, "application/json")
                };
            }
        }
    }
}


