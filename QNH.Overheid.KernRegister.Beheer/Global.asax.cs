using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using QNH.Overheid.KernRegister.Beheer.App_Start;
using QNH.Overheid.KernRegister.Beheer.DependencyResolution;
using QNH.Overheid.KernRegister.Beheer.Utilities;

namespace QNH.Overheid.KernRegister.Beheer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            CultureConfig.EnsureRegisteredUiCulture(this);

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AuthConfig.RegisterAuth();

            GlobalConfiguration.Configuration.Services
                .Replace(typeof(IHttpControllerActivator), new StructureMapHttpControllerActivator(IocConfig.Container));

            // Use XmlSerializer
            //GlobalConfiguration.Configuration.Formatters.XmlFormatter.UseXmlSerializer = true;
            GlobalConfiguration.Configuration.Formatters.Insert(0, new KvKTypesXmlFormatter());
        }
    }
}