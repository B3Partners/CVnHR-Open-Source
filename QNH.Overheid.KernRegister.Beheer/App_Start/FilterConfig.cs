using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using QNH.Overheid.KernRegister.Beheer.Controllers;
using QNH.Overheid.KernRegister.Beheer.Filters;
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

            // Add main authorization attribute to allways check if user has any type of access
            filters.Add(new CVnHRAuthorizeAttribute());

            // Check configuration whether or not the current user has to be authenticated.
            if (SettingsHelper.EnsureAuthenticatedUser)
                filters.Add(new AuthorizeSimpleFilter());
        }
    }
}