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
        }

        private bool Authorize(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.Request.Headers.Contains("Authorization"))
                {
                    string token = actionContext.Request.Headers.GetValues("Authorization").First();

                    using (MeiHi.Model.MeiHiEntities db = new MeiHi.Model.MeiHiEntities())
                    {
                        return db.User.Any(r => r.Token == token);
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}