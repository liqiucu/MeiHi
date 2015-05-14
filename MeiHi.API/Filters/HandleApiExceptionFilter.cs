using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace MeiHi.API.Filters
{
    public class HandleApiExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception.Message == "该授权请求被拒绝")
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": -2, \"errorMessage\": \"" + context.Exception.Message + "\"}", Encoding.UTF8, "application/json")
                };
            }
            else
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": -1, \"errorMessage\": \"" + context.Exception.Message + "\"}", Encoding.UTF8, "application/json")
                };
            }
        }
    }
}