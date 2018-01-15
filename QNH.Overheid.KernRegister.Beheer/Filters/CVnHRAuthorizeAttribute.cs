using QNH.Overheid.KernRegister.Beheer.Utilities;
using QNH.Overheid.KernRegister.Business.Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
    [CVnHRAuthorize(ApplicationActions.CVnHR_Admin)]
    public class CVnHRAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private ApplicationActions[] _actions;
        private static readonly ApplicationActions[] _allActions 
            = Enum.GetValues(typeof(ApplicationActions)).OfType<ApplicationActions>().ToArray();

        public CVnHRAuthorizeAttribute(params ApplicationActions[] actions)
        {
            _actions = actions;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.User;
            var authorized = _actions.Any()
                ? user.IsAllowedAllActions(_actions)
                : user.IsAllowedAnyActions(_allActions);

            // Only check if this user is set as initialAdministrator in AppSettings.config
            // when not authorized.
            if (!authorized && httpContext.User.IsInitialAdmin())
            {
                // Only allow pages without authorization and Admin pages to the initialAdministrators.
                return !_actions.Any() 
                    || _actions.All(a=> a == ApplicationActions.CVnHR_Admin);
            }
            else
            {
                return authorized;
            }
        }
    }
}