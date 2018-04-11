using System.Web.Mvc;

namespace QNH.Overheid.KernRegister.Beheer.Filters
{
    public class IEEnsureCorrectVersionHeaderFilter : ActionFilterAttribute, IActionFilter
    {
        public new void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.HttpContext.Response.IsRequestBeingRedirected)
            {
                // Add the httpheader for IE to ensure correct rendering
                filterContext.HttpContext.Response.Headers.Set("X-UA-Compatible", "IE=edge,chrome=1");

                // Ensure no cache header for IE not to cache all json data
                filterContext.HttpContext.Response.Headers.Set("Cache-Control", "no-cache");
            }
        }

        public new void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //throw new System.NotImplementedException();
        }
    }
}