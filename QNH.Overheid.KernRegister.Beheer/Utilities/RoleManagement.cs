using QNH.Overheid.KernRegister.Beheer.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using NHibernate.Util;
using QNH.Overheid.KernRegister.Business.Service.Users;

namespace QNH.Overheid.KernRegister.Beheer.Utilities
{
    public static class RoleManagement
    {
        private static IUserManager UserManager => IocConfig.Container.GetInstance<IUserManager>();

        public static bool IsAllowedAllActions(this IPrincipal user, params ApplicationActions[] actions)
        {
            return UserManager.IsAllowedAllActions(user.Identity.Name, actions);
        }

        public static bool IsAllowedAnyActions(this IPrincipal user, params ApplicationActions[] actions)
        {
            return UserManager.IsAllowedAnyActions(user.Identity.Name, actions);
        }
    }
}