using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using QNH.Overheid.KernRegister.Beheer.Utilities;
using QNH.Overheid.KernRegister.Business.Service.Users;

namespace QNH.Overheid.KernRegister.Beheer
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new IEEnsureCorrectVersionHeaderFilter());

            // Check configuration whether or not the current user has to be authenticated.
            if (AuthorizeSimpleFilter.EnsureAuthenticatedUser)
                filters.Add(new AuthorizeSimpleFilter());

            if(System.Web.Security.Roles.Enabled)
                filters.Add(new EnsureUserRolesFilter());
        }
    }

    // TODO: extract and rename
    public class IEEnsureCorrectVersionHeaderFilter : ActionFilterAttribute, IActionFilter
    {
        public new void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Add the httpheader for IE to ensure correct rendering
            filterContext.HttpContext.Response.Headers.Set("X-UA-Compatible", "IE=edge,chrome=1");
            
            // Ensure no cache header for IE not to cache all json data
            filterContext.HttpContext.Response.Headers.Set("Cache-Control", "no-cache");
        }

        public new void OnActionExecuting(ActionExecutingContext filterContext)
        {
 	        //throw new System.NotImplementedException();
        }
    }

    public class AuthorizeSimpleFilter : ActionFilterAttribute, IActionFilter
    {
        public static bool EnsureAuthenticatedUser => Convert.ToBoolean(ConfigurationManager.AppSettings["EnsureAuthenticatedUser"]);

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

    public class EnsureUserRolesFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = filterContext.HttpContext.User;

            // Check if the user either may view or manage the page (or is admin)
            if(!user.IsAllowedAnyActions(ApplicationActions.CVnHR_ViewKvKData, 
                ApplicationActions.CVnHR_ManageKvKData, 
                ApplicationActions.CVnHR_Admin))
                HandleUnauthorizedRequest(filterContext);
            else
                base.OnAuthorization(filterContext);
        }
    }
}