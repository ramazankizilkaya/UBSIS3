using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UBSIS3.Web.Data
{
    public class AllowSameSiteAttribute : ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    var response = filterContext.HttpContext.Response;

        //    if (response != null)
        //    {
        //        response.AddHeader("Set-Cookie", "HttpOnly;Secure;SameSite=Strict");
        //        //Add more headers...
        //    }

        //    base.OnActionExecuting(filterContext);
        //}
    }
}
