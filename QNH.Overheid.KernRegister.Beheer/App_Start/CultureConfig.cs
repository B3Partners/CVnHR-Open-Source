using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Web;
using QNH.Overheid.KernRegister.Organization.Resources;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using QNH.Overheid.KernRegister.Business;

namespace QNH.Overheid.KernRegister.Beheer.App_Start
{
    public class CultureConfig
    {
        public static void EnsureRegisteredUiCulture(MvcApplication app)
        {
            var organizationToUse = ConfigurationManager.AppSettings["OrganizationToUse"];

            switch (organizationToUse.ToMaxChars(5).ToLower())
            {
                case "drent":
                    Default.CurrentOrganization = Organization.Resources.Organization.Drenthe;
                    break;
                case "flevo":
                    Default.CurrentOrganization = Organization.Resources.Organization.Flevoland;
                    break;
                default:
                    Default.CurrentOrganization = Organization.Resources.Organization.Qnh;
                    break;
            }
        }
    }

}