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
    [RoutePrefix("company")]
    public class CompanyController : ApiController
    {
        private SiTuXiaoYuanEntities db = new SiTuXiaoYuanEntities();
        [ApiAuthorize]
        [HttpGet]
        public object Get()
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);

            var info = db.api_get_company_info(login.CompanyId).First();
            return new
            {
                jsonStatus = 1,
                companyId = info.CompanyId,
                mobile = info.Mobile,
                fullName = info.FullName ?? "",
                address = info.Address ?? "",
                introduction = info.Introduction ?? "",
                balance = info.Balance,
                licensePhoto = info.LicensePhoto,
                agentPhoto = info.AgentPhoto,
                verified = info.Verified
            };
        }

        [Route("{id}")]
        [HttpGet]
        public object GetCompany(long id)
        {
            var info = db.api_get_company_info(id).FirstOrDefault();
            if (info != null)
            {
                return new
                {
                    jsonStatus = 1,
                    companyId = info.CompanyId,
                    mobile = info.Mobile,
                    fullName = info.FullName ?? "",
                    address = info.Address ?? "",
                    introduction = info.Introduction ?? "",
                    verified = info.Verified
                };
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 0, \"alertMessage\": \"该企业不存在\"}", Encoding.UTF8, "application/json")
                };
            }
        }

        [ApiAuthorize]
        [Route("update")]
        [HttpPost]
        public object Update([FromBody]UpdateCompanyInfo updateCompanyInfo)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);

            var info = db.api_update_company_info(login.CompanyId, updateCompanyInfo.Mobile, updateCompanyInfo.FullName, updateCompanyInfo.Address, updateCompanyInfo.Introduction).First();

            return new
            {
                jsonStatus = 1,
                companyId = info.CompanyId,
                mobile = info.Mobile,
                fullName = info.FullName ?? "",
                address = info.Address ?? "",
                introduction = info.Introduction ?? "",
                balance = info.Balance,
                licensePhoto = info.LicensePhoto,
                agentPhoto = info.AgentPhoto,
                verified = info.Verified
            };
        }

        [ApiAuthorize]
        [Route("upload_certificate")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadCertificate()
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
                var login = db.CompanyLogons.Single(r => r.Token == token);
                var company = db.Companies.Single(r => r.CompanyId == login.CompanyId);

                // 文件在服务端的保存地址，需要的话自行 rename 或 move
                string extension = ".jpg";
                string newPath = "/upload/company/" + login.CompanyId + "/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";

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
                        company.LicensePhoto = newPath + fileName + extension;
                    }
                    else if (i == 1)
                    {
                        company.AgentPhoto = newPath + fileName + extension;
                    }
                    i++;
                }
                company.DateModified = DateTime.Now;
                db.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 1, \"licensePhoto\": \"" + company.LicensePhoto + "\", \"AgentPhoto\": \"" + company.LicensePhoto + "\"}", Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": -1,\"errorMessage\": \"" + ex.Message + "\"}", Encoding.UTF8, "application/json")
                };
            }
        }

        [ApiAuthorize]
        [Route("job_hire/{id}")]
        [HttpPost]
        public HttpResponseMessage JobHire(long id, [FromBody]ApplyModel applyModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
            string message = db.api_job_company_hire(login.CompanyId, id, applyModel.UserId, applyModel.JobTypeId).First();

            if (!string.IsNullOrEmpty(message))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 0, \"alertMessage\": \"" + message + "\" }", Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jsonStatus\": 1 }", Encoding.UTF8, "application/json")
                };
            }
        }

        [ApiAuthorize]
        [Route("job_reject/{id}")]
        [HttpPost]
        public HttpResponseMessage JobReject(long id, [FromBody]ApplyModel applyModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
            db.api_job_company_reject(login.CompanyId, id, applyModel.UserId, applyModel.JobTypeId);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"jsonStatus\": 1 }", Encoding.UTF8, "application/json")
            };
        }

        [Route("job_applied_users/{id}")]
        [ApiAuthorize]
        [HttpPost]
        public object JobAppliedUsers(long id, [FromBody]JobTypeModel jobTypeModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
            switch (jobTypeModel.JobTypeId)
            {
                case 1:
                    var r1 = db.api_get_job_applied_user_activity(id, login.CompanyId);
                    if (r1 != null)
                        return new { jsonStatus = 1, results = r1 };
                    break;
                case 2:
                    var r2 = db.api_get_job_applied_user_campus(id, login.CompanyId);
                    if (r2 != null)
                        return new { jsonStatus = 1, results = r2 };
                    break;
                case 3:
                    var r3 = db.api_get_job_applied_user_intern(id, login.CompanyId);
                    if (r3 != null)
                        return new { jsonStatus = 1, results = r3 };
                    break;
                case 4:
                    var r4 = db.api_get_job_applied_user_service(id, login.CompanyId);
                    if (r4 != null)
                        return new { jsonStatus = 1, results = r4 };
                    break;
                case 5:
                    var r5 = db.api_get_job_applied_user_tutor(id, login.CompanyId);
                    if (r5 != null)
                        return new { jsonStatus = 1, results = r5 };
                    break;
                default:
                    break;
            }
            return new { jsonStatus = 1, results = new { } };
        }

        [Route("job_list")]
        [ApiAuthorize]
        [HttpGet]
        public object JobList()
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);

            return new { jsonStatus = 1, results = db.api_get_company_job_list(login.CompanyId) };
        }

        [Route("publish_activity_job")]
        [ApiAuthorize]
        [HttpPost]
        public object PublishActivityJob([FromBody]PublishActivityJobModel publishActivityJobModel)
        {
            if (publishActivityJobModel.DateExpiry > DateTime.Today.AddMonths(3))
            {
                return new { jsonStatus = 0, alertMessage = "岗位发布截至日期不得超过3个月" };
            }
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
            //db.api_publish_activity_job(login.CompanyId, publishActivityJobModel.Title, publishActivityJobModel.PositionId, publishActivityJobModel.RegionId, publishActivityJobModel.WageUnitId, publishActivityJobModel.Wage,
            //    publishActivityJobModel.IncludeDinner, publishActivityJobModel.IncludeRoom, publishActivityJobModel.TermId, publishActivityJobModel.Gender == "男" ? 1 : (publishActivityJobModel.Gender == "女" ? 0 : (int?)null),
            //    publishActivityJobModel.PeopleRequired, publishActivityJobModel.Contact, publishActivityJobModel.ContactPhone, publishActivityJobModel.Address, publishActivityJobModel.DateFrom, publishActivityJobModel.DateExpiry, publishActivityJobModel.WorkFrom,
            //    publishActivityJobModel.WorkEnd, publishActivityJobModel.Description, publishActivityJobModel.MonMorning, publishActivityJobModel.MonAfternoon, publishActivityJobModel.MonNight,
            //    publishActivityJobModel.TueMorning, publishActivityJobModel.TueAfternoon, publishActivityJobModel.TueNight, publishActivityJobModel.WedMorning, publishActivityJobModel.WedAfternoon,
            //    publishActivityJobModel.WedNight, publishActivityJobModel.ThuMorning, publishActivityJobModel.ThuAfternoon, publishActivityJobModel.ThuNight, publishActivityJobModel.FriMorning,
            //    publishActivityJobModel.FriAfternoon, publishActivityJobModel.FriNight, publishActivityJobModel.SatMorning, publishActivityJobModel.SatAfternoon, publishActivityJobModel.SatNight,
            //    publishActivityJobModel.SunMorning, publishActivityJobModel.SunAfternoon, publishActivityJobModel.SunNight);

            return new { jsonStatus = 1 };
        }

        [Route("publish_campus_job")]
        [ApiAuthorize]
        [HttpPost]
        public object PublishCampusJob([FromBody]PublishCampusJobModel publishCampusJobModel)
        {
            if (publishCampusJobModel.DateExpiry > DateTime.Today.AddMonths(3))
            {
                return new { jsonStatus = 0, alertMessage = "岗位发布截至日期不得超过3个月" };
            }
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
            db.api_publish_campus_job(login.CompanyId, publishCampusJobModel.Title, publishCampusJobModel.CategoryId, publishCampusJobModel.SchoolId, publishCampusJobModel.RegionId,
                publishCampusJobModel.Gender == "男" ? 1 : (publishCampusJobModel.Gender == "女" ? 0 : (int?)null),
                publishCampusJobModel.PeopleRequired, publishCampusJobModel.Contact, publishCampusJobModel.ContactPhone, publishCampusJobModel.Address, publishCampusJobModel.DateFrom, publishCampusJobModel.DateExpiry, publishCampusJobModel.WorkFrom,
                publishCampusJobModel.WorkEnd, publishCampusJobModel.Description, publishCampusJobModel.MonMorning, publishCampusJobModel.MonAfternoon, publishCampusJobModel.MonNight,
                publishCampusJobModel.TueMorning, publishCampusJobModel.TueAfternoon, publishCampusJobModel.TueNight, publishCampusJobModel.WedMorning, publishCampusJobModel.WedAfternoon,
                publishCampusJobModel.WedNight, publishCampusJobModel.ThuMorning, publishCampusJobModel.ThuAfternoon, publishCampusJobModel.ThuNight, publishCampusJobModel.FriMorning,
                publishCampusJobModel.FriAfternoon, publishCampusJobModel.FriNight, publishCampusJobModel.SatMorning, publishCampusJobModel.SatAfternoon, publishCampusJobModel.SatNight,
                publishCampusJobModel.SunMorning, publishCampusJobModel.SunAfternoon, publishCampusJobModel.SunNight);

            return new { jsonStatus = 1 };
        }

        [Route("publish_intern_job")]
        [ApiAuthorize]
        [HttpPost]
        public object PublishInternJob([FromBody]PublishInternJobModel publishInternJobModel)
        {
            if (publishInternJobModel.DateExpiry > DateTime.Today.AddMonths(3))
            {
                return new { jsonStatus = 0, alertMessage = "岗位发布截至日期不得超过3个月" };
            }
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
            //db.api_publish_intern_job(login.CompanyId, publishInternJobModel.Title, publishInternJobModel.CategoryId, publishInternJobModel.RegionId, publishInternJobModel.WageUnitId, publishInternJobModel.Wage,
            //    publishInternJobModel.IncludeDinner, publishInternJobModel.IncludeRoom, publishInternJobModel.Gender == "男" ? 1 : (publishInternJobModel.Gender == "女" ? 0 : (int?)null),
            //    publishInternJobModel.PeopleRequired, publishInternJobModel.Contact, publishInternJobModel.ContactPhone, publishInternJobModel.Address, publishInternJobModel.DateFrom, publishInternJobModel.DateExpiry, publishInternJobModel.WorkFrom,
            //    publishInternJobModel.WorkEnd, publishInternJobModel.Description, publishInternJobModel.MonMorning, publishInternJobModel.MonAfternoon, publishInternJobModel.MonNight,
            //    publishInternJobModel.TueMorning, publishInternJobModel.TueAfternoon, publishInternJobModel.TueNight, publishInternJobModel.WedMorning, publishInternJobModel.WedAfternoon,
            //    publishInternJobModel.WedNight, publishInternJobModel.ThuMorning, publishInternJobModel.ThuAfternoon, publishInternJobModel.ThuNight, publishInternJobModel.FriMorning,
            //    publishInternJobModel.FriAfternoon, publishInternJobModel.FriNight, publishInternJobModel.SatMorning, publishInternJobModel.SatAfternoon, publishInternJobModel.SatNight,
            //    publishInternJobModel.SunMorning, publishInternJobModel.SunAfternoon, publishInternJobModel.SunNight);

            return new { jsonStatus = 1 };
        }

        [Route("publish_service_job")]
        [ApiAuthorize]
        [HttpPost]
        public object PublishServiceJob([FromBody]PublishServiceJobModel publishServiceJobModel)
        {
            if (publishServiceJobModel.DateExpiry > DateTime.Today.AddMonths(3))
            {
                return new { jsonStatus = 0, alertMessage = "岗位发布截至日期不得超过3个月" };
            }
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
            //db.api_publish_service_job(login.CompanyId, publishServiceJobModel.Title, publishServiceJobModel.CategoryId, publishServiceJobModel.PositionId, publishServiceJobModel.RegionId, publishServiceJobModel.WageUnitId, publishServiceJobModel.Wage,
            //    publishServiceJobModel.IncludeDinner, publishServiceJobModel.IncludeRoom, publishServiceJobModel.TermId, publishServiceJobModel.Gender == "男" ? 1 : (publishServiceJobModel.Gender == "女" ? 0 : (int?)null),
            //    publishServiceJobModel.PeopleRequired, publishServiceJobModel.Contact, publishServiceJobModel.ContactPhone, publishServiceJobModel.Address, publishServiceJobModel.DateFrom, publishServiceJobModel.DateExpiry, publishServiceJobModel.WorkFrom,
            //    publishServiceJobModel.WorkEnd, publishServiceJobModel.Description, publishServiceJobModel.MonMorning, publishServiceJobModel.MonAfternoon, publishServiceJobModel.MonNight,
            //    publishServiceJobModel.TueMorning, publishServiceJobModel.TueAfternoon, publishServiceJobModel.TueNight, publishServiceJobModel.WedMorning, publishServiceJobModel.WedAfternoon,
            //    publishServiceJobModel.WedNight, publishServiceJobModel.ThuMorning, publishServiceJobModel.ThuAfternoon, publishServiceJobModel.ThuNight, publishServiceJobModel.FriMorning,
            //    publishServiceJobModel.FriAfternoon, publishServiceJobModel.FriNight, publishServiceJobModel.SatMorning, publishServiceJobModel.SatAfternoon, publishServiceJobModel.SatNight,
            //    publishServiceJobModel.SunMorning, publishServiceJobModel.SunAfternoon, publishServiceJobModel.SunNight);

            return new { jsonStatus = 1 };
        }

        [Route("publish_tutor_job")]
        [ApiAuthorize]
        [HttpPost]
        public object PublishTutorJob([FromBody]PublishTutorJobModel publishTutorJobModel)
        {
            if (publishTutorJobModel.DateExpiry > DateTime.Today.AddMonths(3))
            {
                return new { jsonStatus = 0, alertMessage = "岗位发布截至日期不得超过3个月" };
            }
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.SingleOrDefault(r => r.Token == token);
            //db.api_publish_tutor_job(login.CompanyId, publishTutorJobModel.Title, publishTutorJobModel.LevelId, publishTutorJobModel.PaperId, publishTutorJobModel.RegionId, publishTutorJobModel.WageUnitId, publishTutorJobModel.Wage,
            //    publishTutorJobModel.IncludeDinner, publishTutorJobModel.IncludeRoom, publishTutorJobModel.TermId, publishTutorJobModel.Gender == "男" ? 1 : (publishTutorJobModel.Gender == "女" ? 0 : (int?)null),
            //    publishTutorJobModel.PeopleRequired, publishTutorJobModel.Contact, publishTutorJobModel.ContactPhone, publishTutorJobModel.Address, publishTutorJobModel.DateFrom, publishTutorJobModel.DateExpiry, publishTutorJobModel.WorkFrom,
            //    publishTutorJobModel.WorkEnd, publishTutorJobModel.Description, publishTutorJobModel.MonMorning, publishTutorJobModel.MonAfternoon, publishTutorJobModel.MonNight,
            //    publishTutorJobModel.TueMorning, publishTutorJobModel.TueAfternoon, publishTutorJobModel.TueNight, publishTutorJobModel.WedMorning, publishTutorJobModel.WedAfternoon,
            //    publishTutorJobModel.WedNight, publishTutorJobModel.ThuMorning, publishTutorJobModel.ThuAfternoon, publishTutorJobModel.ThuNight, publishTutorJobModel.FriMorning,
            //    publishTutorJobModel.FriAfternoon, publishTutorJobModel.FriNight, publishTutorJobModel.SatMorning, publishTutorJobModel.SatAfternoon, publishTutorJobModel.SatNight,
            //    publishTutorJobModel.SunMorning, publishTutorJobModel.SunAfternoon, publishTutorJobModel.SunNight);

            return new { jsonStatus = 1 };
        }

        [ApiAuthorize]
        [Route("change_password")]
        [HttpPost]
        public HttpResponseMessage ChangePassword([FromBody]ChangePasswordModel changePasswordModel)
        {
            string token = Request.Headers.GetValues("Authorization").First();
            var login = db.CompanyLogons.Single(r => r.Token == token);
            var company = db.Companies.Single(r => r.CompanyId == login.CompanyId);
            string salt = company.Salt;
            string hashedPassword = Helper.GenerateHashWithSalt(changePasswordModel.OldPassword, salt);
            if (hashedPassword == company.Password)
            {
                login.Token = Guid.NewGuid().ToString();
                login.TokenExpiryDate = DateTime.Now.AddMonths(1);
                login.IPAddress = ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                login.DateLogon = DateTime.Now;
                db.SaveChanges();

                salt = Guid.NewGuid().ToString();
                hashedPassword = Helper.GenerateHashWithSalt(changePasswordModel.NewPassword, salt);
                company.Salt = salt;
                company.Password = hashedPassword;
                company.DateModified = DateTime.Now;
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
