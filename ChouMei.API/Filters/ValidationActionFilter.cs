using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;

namespace ChouMei.API.Filters
{
    public class ValidationActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            //if (actionContext.Request.Method == HttpMethod.Post && actionContext.Request.Content.Headers.ContentLength == 0)
            //{
            //    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, "{errorMessage: 不允许空数据提交}", System.Net.Http.Formatting.JsonMediaTypeFormatter.DefaultMediaType);
            //    return;
            //}

            var modelState = actionContext.ModelState;
            if (!modelState.IsValid)
            {
                string errors = string.Empty;
                foreach (var key in modelState.Keys)
                {
                    var state = modelState[key];
                    if (state.Errors.Any())
                    {
                        errors = errors + state.Errors.First().ErrorMessage + "\\n";
                    }
                }
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"jsonStatus\": 0, \"alertMessage\": \"" + errors + "\"}", Encoding.UTF8, "application/json")
                };
                //actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                //{
                //    Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(modelState))
                //};
            }
        }
    }
}