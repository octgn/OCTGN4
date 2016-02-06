using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using Owin;
using Ninject;
using Microsoft.AspNet.SignalR.Hubs;
using Octgn.Shared;
using Microsoft.Owin.Extensions;

[assembly: OwinStartup(typeof (Octgn.UI.OwinStartup))]
namespace Octgn.UI
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
			LoggerFactory.SetDefault<HubLogger>();

			var settings = new NinjectSettings();
			settings.LoadExtensions = false;
            var applicationLifetimeKernel = new StandardKernel(settings);
            applicationLifetimeKernel.Bind<LocalServerManager>().ToSelf().InSingletonScope();
            applicationLifetimeKernel.Bind<UserSessions>().ToSelf().InSingletonScope();

            // Register hubs
            //applicationLifetimeKernel.Bind<Octgn.UI.MainHub>().ToSelf();
            //applicationLifetimeKernel.Bind<Octgn.UI.HubLogger>().ToSelf();

            app.Use<SetPrincipalOwinMiddleware>(app, applicationLifetimeKernel);
            app.Use<LoggerOwinMiddleware>(app);
            app.Map("/signalr", map =>
                {
                    var activator = new OctgnHubActivator(applicationLifetimeKernel);
                    map.UseCors(CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration();
                    hubConfiguration.Resolver.Register(typeof(IHubActivator), () => activator);
                    hubConfiguration.Resolver.Resolve<IHubPipeline>().AddModule(new Modules.SignalrPipelineModule(applicationLifetimeKernel));
                    hubConfiguration.EnableDetailedErrors = true;

                    map.RunSignalR(hubConfiguration);
                }
            );
            app.UseNancy(op =>
                {
                    op.Bootstrapper = new NancyBootstrapper(applicationLifetimeKernel);
                }
            );
			app.UseStageMarker(PipelineStage.MapHandler);
        }
    }

    public class OctgnHubActivator : IHubActivator
    {
        private IKernel _kernel;
        public OctgnHubActivator(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            return (IHub)_kernel.Get(descriptor.HubType);
        }
    }
}