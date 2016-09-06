using System.Web;
using System.Web.Mvc;

namespace CBLSummer09052016Budgeter
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
