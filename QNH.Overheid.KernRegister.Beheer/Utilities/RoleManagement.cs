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
            return UserManager.IsAllowedAllActions(user.GetUserName(), actions);
        }

        public static bool IsAllowedAnyActions(this IPrincipal user, params ApplicationActions[] actions)
        {
            return UserManager.IsAllowedAnyActions(user.GetUserName(), actions);
        }

        public static string GetUserName(this IPrincipal user)
        {
            var username = user.Identity.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                if (SettingsHelper.UsernameToUseWhenEmpty == null)
                    throw new ArgumentException("username cannot be null or whitespace. Set the application setting 'UsernameToUseWhenEmpty' in /Config/AppSettings.config");
                else
                    username = SettingsHelper.UsernameToUseWhenEmpty;
            }
            return username;
        }

        public static bool IsInitialAdmin(this IPrincipal user)
        {
            return SettingsHelper.InitialUserAdministrators.Contains(user.GetUserName(), StringComparer.InvariantCultureIgnoreCase);
        }
    }
}