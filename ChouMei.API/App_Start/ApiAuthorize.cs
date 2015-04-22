using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
namespace System.Web.Http
{
    public class ApiAuthorize : System.Web.Http.AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {

            if (Authorize(actionContext))
            {
                return;
            }
            throw new Exception("该授权请求被拒绝");
            //base.OnAuthorization(actionContext);
        }

        private bool Authorize(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.Request.Headers.Contains("Authorization"))
                {
                    string token = actionContext.Request.Headers.GetValues("Authorization").First();
                    ChouMei.Model.SiTuXiaoYuanEntities db = new ChouMei.Model.SiTuXiaoYuanEntities();
                    return db.UserLogons.Any(r => r.Token == token) || db.CompanyLogons.Any(r => r.Token == token);
                    //boolean logic to determine if you are authorized.  
                    //We check for a valid token in the request header or cookie.
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}