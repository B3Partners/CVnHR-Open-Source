using QNH.Overheid.KernRegister.Beheer.Utilities;
using QNH.Overheid.KernRegister.Business.Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

using System.DirectoryServices.AccountManagement;
using NLog;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    [CVnHRAuthorize(ApplicationActions.CVnHR_Admin)]
    public class CVnHRAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly ApplicationActions[] _actions;
        private readonly bool _anyActions;

        private static readonly ApplicationActions[] _allActions 
            = Enum.GetValues(typeof(ApplicationActions)).OfType<ApplicationActions>().ToArray();

        private static readonly Logger Log = LogManager.GetLogger("authorizationLogger");

        public CVnHRAuthorizeAttribute(params ApplicationActions[] actions)
        {
            _actions = actions;
            _anyActions = false;
        }

        public CVnHRAuthorizeAttribute(bool anyActions, params ApplicationActions[] actions)
        {
            _actions = actions;
            _anyActions = anyActions;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.User;
            var authorized = false;

            if (!user.UserMatchesAnyADDistinguishedNameFilter())
            {
                authorized = false;
            }
            else
            {
                authorized = _actions.Any()
                    ? (_anyActions ? user.IsAllowedAnyActions(_actions) : user.IsAllowedAllActions(_actions))
                    : user.IsAllowedAnyActions(_allActions);

                // Only check if this user is set as initialAdministrator in AppSettings.config
                // when not authorized.
                if (!authorized && httpContext.User.IsInitialAdmin())
                {
                    // Only allow pages without authorization and Admin pages to the initialAdministrators.
                    authorized = !_actions.Any()
                        || _actions.All(a => a == ApplicationActions.CVnHR_Admin);
                }
            }

            Log.Info($"Request: {httpContext.Request.RawUrl} - user: {user.GetUserName()} - AUTHORIZED: {authorized}");
            return authorized;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            Log.Warn($"Request: {filterContext.RequestContext.HttpContext.Request.RawUrl} - user: {filterContext.HttpContext.User.GetUserName()} - UNAUTHORIZED");
            filterContext.HttpContext.Response.Redirect($"~/Users/AccessDenied?actions={string.Join(",",_actions)}&any={_anyActions}");
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}