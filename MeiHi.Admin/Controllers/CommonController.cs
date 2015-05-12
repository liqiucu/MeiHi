
using MeiHi.Admin.Helper;
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

namespace MeiHi.Admin.Controllers
{
    public class CommonController : ApiController
    {
        [Route("upload_productbrand")]
        public async Task<HttpResponseMessage> UploadProductBrand()
        {
            try
            {
                // 设置上传目录
                string tempPath = "/upload/temp/";
                var provider = new MultipartFormDataStreamProvider(System.Web.HttpContext.Current.Server.MapPath(tempPath));

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

                //string token = Request.Headers.GetValues("Authorization").First();
                //var login = db.UserLogons.Single(r => r.Token == token);
                //var user = db.Users.Single(r => r.UserId == login.UserId);

                // 文件在服务端的保存地址，需要的话自行 rename 或 move
                string extension = ".jpg";
                string shopId = "321";
                string newPath = "/upload/shop/" + shopId + "/";

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
                        //user.ProfilePhoto = newPath + fileName + extension;
                    }
                    i++;
                }
                //user.DateModified = DateTime.Now;
                //db.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                   // Content = new StringContent("{\"jsonStatus\": 1, \"profilePhoto\": \"" + newPath + fileName + extension + "\" }", Encoding.UTF8, "application/json")
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
    }
}
