using ChouMei.API.Filters;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace ChouMei.API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterHttpFilters(HttpFilterCollection filters)
        {
            filters.Add(new ValidationActionFilter());
        }
    }
}
