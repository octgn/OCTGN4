using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using Owin;
using Ninject;

[assembly: OwinStartup(typeof (Octgn.UI.OwinStartup))]
namespace Octgn.UI
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            //LoggerFactory.SetDefault<Logger>();

            var applicationLifetimeKernel = new StandardKernel();
            applicationLifetimeKernel.Bind<LocalServerManager>().ToSelf().InSingletonScope();
            app.Map("/signalr", map =>
                {
                    map.UseCors(CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration();
                    hubConfiguration.Resolver = new NinjectSignalRDependencyResolver(applicationLifetimeKernel);
                    map.RunSignalR(hubConfiguration);
                }
            );
            app.UseNancy(op =>
                {
                    op.Bootstrapper = new NancyBootstrapper(applicationLifetimeKernel);
                }
            );
            app.Use<LoggerOwinMiddleware>(app);
            GlobalHost.HubPipeline.AddModule(new Modules.CallerCulturePipelineModule());
        }
    }
}