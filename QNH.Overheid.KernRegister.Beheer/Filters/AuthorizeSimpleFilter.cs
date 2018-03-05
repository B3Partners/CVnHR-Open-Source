using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace QNH.Overheid.KernRegister.Beheer.Filters
{
    public class AuthorizeSimpleFilter : ActionFilterAttribute, IActionFilter
    {
        public new void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new System.NotImplementedException();
        }

        public new void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.RouteData.Values.ContainsValue("Login") && !filterContext.IsChildAction)
            {
                if (!filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    //filterContext.HttpContext.Response.RedirectToRoute("Account/Login");
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
                }
            }
            else
                base.OnActionExecuting(filterContext);
        }
    }
}