using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Localization;
using System.Globalization;
using Octgn.UI.Middleware;

namespace Octgn.UI
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<LocalServerManager>();
            services.AddSingleton<UserSessions>();

            //services.AddLocalization(x => x.ResourcesPath = "Resources");
            //services.AddMvc().AddDataAnnotationsLocalization();
            services.AddMvc();  

            //TODO Reimplement all this stuff
            //services.AddSignalR(opts => {

            //});

            // Configure supported cultures and localization options
            //services.Configure<RequestLocalizationOptions>(options =>
            //{
            //    var supportedCultures = new[]
            //    {
            //        new CultureInfo("en-US"),
            //        new CultureInfo("es")
            //    };

            //    // State what the default culture for your application is. This will be used if no specific culture
            //    // can be determined for a given request.
            //    //options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");

            //    // You must explicitly state which cultures your application supports.
            //    // These are the cultures the app supports for formatting numbers, dates, etc.
            //    options.SupportedCultures = supportedCultures;

            //    // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
            //    options.SupportedUICultures = supportedCultures;

            //    // You can change which providers are configured to determine the culture for requests, or even add a custom
            //    // provider with your own logic. The providers will be asked in order to provide a culture for each request,
            //    // and the first to provide a non-null result that is in the configured supported cultures list will be used.
            //    // By default, the following built-in providers are configured:
            //    // - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
            //    // - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
            //    // - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header
            //    //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
            //    //{
            //    //  // My custom request culture logic
            //    //  return new ProviderCultureResult("en");
            //    //}));
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //app.UseRequestLocalization(new RequestCulture(culture: "en-US", uiCulture: "en-US"));
            //app.Map("/signalr", map =>
            //    {
            //        map.UseCors("AllowAll");
            //        //var hubConfiguration = new HubConfiguration();
            //        //hubConfiguration.Resolver.Register(typeof(IHubActivator), () => activator);
            //        //hubConfiguration.Resolver.Resolve<IHubPipeline>().AddModule(new Modules.SignalrPipelineModule(applicationLifetimeKernel));

            //        //map.RunSignalR(hubConfiguration);
            //        map.RunSignalR();
            //    }
            //);
            //app.UseSignalR();
            //app.UseAuthenticationMiddleware();
            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc(r => r.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                )
            );
            //app.Use(req => {
            //_logger.WriteVerbose(string.Format("{0} {1}: {2}", context.Request.Scheme, context.Request.Method, context.Request.Path));
            //await Next.Invoke(context);
            //});
        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            WebApplication.Run<Startup>(args);
        }
    }
}
