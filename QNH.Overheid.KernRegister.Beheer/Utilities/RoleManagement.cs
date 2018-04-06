using NHibernate.Util;
using QNH.Overheid.KernRegister.Business.Service.Users;
using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;

namespace QNH.Overheid.KernRegister.Beheer.Utilities
{
    public static class RoleManagement
    {
        public static bool IsAllowedAllActions(this IPrincipal user, params ApplicationActions[] actions)
        {
            using (var userManager = IocConfig.Container.GetInstance<IUserManager>())
            {
                return userManager.IsAllowedAllActions(user.GetUserName(), actions);
            }
        }

        public static bool IsAllowedAnyActions(this IPrincipal user, params ApplicationActions[] actions)
        {
            using (var userManager = IocConfig.Container.GetInstance<IUserManager>())
            {
                return userManager.IsAllowedAnyActions(user.GetUserName(), actions);
            }
        }

        public static bool UserMatchesAnyADDistinguishedNameFilter(this IPrincipal user)
        {
            var dnFilters = SettingsHelper.ADDistinguishedNameFilters;
            if (dnFilters.Any())
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    using (var principal = UserPrincipal.FindByIdentity(context, user.Identity.Name))
                    {
                        var distinguishedName = principal.DistinguishedName;
                        return dnFilters.Any(filter => filter.All(f => distinguishedName.Contains(f)));
                    }
                }
            }

            return true;
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