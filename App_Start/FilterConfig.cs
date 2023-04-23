using System.Web;
using System.Web.Mvc;

namespace Agent_WebForm_Prodject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
