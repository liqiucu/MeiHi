using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MeiHi.Model;
using MeiHi.API.ViewModels;
using System.Threading.Tasks;
using MeiHi.API.Helper;
using Newtonsoft.Json;
using System.Web.Caching;
using MeiHi.API.Logic;

namespace MeiHi.API.Controllers
{
    public class HairStyleController : ApiController
    {
        [Route("get_topHairStyle")]
        [HttpGet]
        public object GetTopHairStyle(int returncounts)
        {
            using (var db = new MeiHiEntities())
            {
                var hairs = db.HairStyleType.Take(returncounts);

                if (hairs == null || hairs.Count() == 0)
                {
                    return new
                    {
                        jsonStatus = 1,
                        resut = "暂时没有发型可供选择"
                    };
                }

                var result = new List<HairStylesModel>();

                foreach (var item in hairs)
                {
                    var styles = new List<HairStyleModel>();

                    foreach (var x in item.HairStyle)
                    {
                        styles.Add(new HairStyleModel()
                        {
                            HairImage = x.HairStyleUrl,
                            HairStyleId = x.HairStyleId,
                            ModelImage = x.HairStyleModelUrl
                        });
                    }

                    result.Add(new HairStylesModel()
                    {
                        TypeId = item.HairStyleTypeId,
                        TypeName = item.Title,
                        HiarStyles = styles
                    });
                }

                return new
                {
                    jsonStatus = 1,
                    resut = result
                }; 
            }
        }
    }
}