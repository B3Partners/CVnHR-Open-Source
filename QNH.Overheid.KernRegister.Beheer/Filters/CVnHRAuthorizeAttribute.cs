using QNH.Overheid.KernRegister.Beheer.Utilities;
using QNH.Overheid.KernRegister.Business.Service.Users;
using System;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace QNH.Overheid.KernRegister.Beheer.Controllers
{
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
            return _actions.Any()
                ? user.IsAllowedAllActions(_actions)
                : user.IsAllowedAnyActions(_allActions);
        }
    }
}