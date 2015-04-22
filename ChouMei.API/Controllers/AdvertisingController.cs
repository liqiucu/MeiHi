using ChouMei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChouMei.API.Controllers
{
    [RoutePrefix("advertising")]
    public class AdvertisingController : ApiController
    {
        private SiTuXiaoYuanEntities db = new SiTuXiaoYuanEntities();
        [Route("get_home_page_advertising")]
        [HttpGet]
        public object GetHomePageAdvertising()
        {
            return new
            {
                jsonStatus = 1,
                results = db.api_get_advertising_home_page()
            };
        }
    }
}
