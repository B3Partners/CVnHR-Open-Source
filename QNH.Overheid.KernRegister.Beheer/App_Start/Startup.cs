using System;
using System.Threading.Tasks;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using SignalR.StructureMap;

[assembly: OwinStartup(typeof(QNH.Overheid.KernRegister.Beheer.Startup))]

namespace QNH.Overheid.KernRegister.Beheer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            var container = IocConfig.Container;

            //SignalR stuff
            var resolver = new SignalRStructureMapDependencyResolver(container);
            var config = new HubConfiguration()
            {
                Resolver = resolver
            };

            // Any connection or hub wire up and configuration should go here
            app.MapSignalR(config);
        }
    }
}
