using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using Owin;
using Ninject;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Octgn.Shared;
using Microsoft.AspNet.SignalR.Messaging;

[assembly: OwinStartup(typeof (Octgn.UI.OwinStartup))]
namespace Octgn.UI
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
			//LoggerFactory.SetDefault<Logger>();
			LoggerFactory.SetDefault<HubLogger>();

			var settings = new NinjectSettings();
			settings.LoadExtensions = false;
            var applicationLifetimeKernel = new StandardKernel(settings);
            applicationLifetimeKernel.Bind<LocalServerManager>().ToSelf().InSingletonScope();
            applicationLifetimeKernel.Bind<UserSessions>().ToSelf().InSingletonScope();

            app.Use<SetPrincipalOwinMiddleware>(app, applicationLifetimeKernel);
            app.Use<LoggerOwinMiddleware>(app);
            app.Map("/signalr", map =>
                {
                    map.UseCors(CorsOptions.AllowAll);
					var current = GlobalHost.DependencyResolver;
                    var hubConfiguration = new HubConfiguration();
                    hubConfiguration.Resolver = new NinjectSignalRDependencyResolver(applicationLifetimeKernel);

                    hubConfiguration.Resolver.Resolve<IHubPipeline>().AddModule(new Modules.SignalrPipelineModule(applicationLifetimeKernel));
                    applicationLifetimeKernel.Bind<IConnectionManager>().ToMethod((x) => hubConfiguration.Resolver.Resolve<IConnectionManager>());
                    map.RunSignalR(hubConfiguration);
                }
            );
            app.UseNancy(op =>
                {
                    op.Bootstrapper = new NancyBootstrapper(applicationLifetimeKernel);
                }
            );
        }
    }
}