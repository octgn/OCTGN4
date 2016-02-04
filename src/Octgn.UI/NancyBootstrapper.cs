using System.Collections.Generic;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using System.Reflection;
using Nancy.Bootstrappers.Ninject;
using Ninject;
using Nancy.Security;
using System;
using Nancy.Owin;
using System.Security.Claims;
using Nancy.Responses;

namespace Octgn.UI
{
    public class NancyBootstrapper : NinjectNancyBootstrapper
    {
        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(x => x.ResourceAssemblyProvider = typeof(CustomResourceAssemblyProvider));
            }
        }

        private IKernel _appKernel;

        public NancyBootstrapper(IKernel kern)
        {
            _appKernel = kern;
        }

        protected override IKernel GetApplicationContainer()
        {
            _appKernel.Load<FactoryModule>();
            return _appKernel;
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts", "Scripts"));
        }

        protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
        {
            pipelines.BeforeRequest += (x) => LoginRedirect(container, x);

            base.ApplicationStartup(container, pipelines);

            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
        }

        protected override void RequestStartup(IKernel container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
        }

        protected Response LoginRedirect(IKernel k, NancyContext ctx)
        {
            if (ctx.Request.Path.ToLower().StartsWith("/login")) return null;
			if(ctx.Request.Query.ContainsKey("sid") == false)
                return new RedirectResponse("/Login");
            if (string.IsNullOrWhiteSpace(ctx.Request.Query.sid))
                return new RedirectResponse("/Login");

            var user = k.Get<UserSessions>().Get(ctx.Request.Query.sid);
            if(user == null)
                return new RedirectResponse("/Login");

            ctx.CurrentUser = user;
            ctx.ViewBag.Sid = user.Sid;

            return null;
        }

        protected override void ConfigureRequestContainer(IKernel container, NancyContext context)
        {
            // Perform registrations that should have a request lifetime
        }
        public class CustomResourceAssemblyProvider : IResourceAssemblyProvider
        {
            private IEnumerable<Assembly> _asses;

            public IEnumerable<Assembly> GetAssembliesToScan()
            {
                return (this._asses ?? (this._asses = GetAsses()));
            }

            private static IEnumerable<Assembly> GetAsses()
            {
                var list = new List<Assembly>(AppDomainAssemblyTypeScanner.Assemblies);
                list.Add(typeof(Octgn.Shared.Resources.Text).Assembly);
                return list;
            }
        }
    }
}