using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Ninject;
using Octgn.Shared.Resources;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;

namespace Octgn.UI.Modules
{
    public class SignalrPipelineModule : HubPipelineModule
    {
        private IKernel _kernel;
        public SignalrPipelineModule(IKernel kernel)
        {
            _kernel = kernel;
        }

        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            // Use the value we stored in the Culture property of the caller's state when they connected
            if (context.Hub.Context.Request.Headers["Accept-Language"] != null)
            {
                var lang = context.Hub.Context.Request.Headers["Accept-Language"].Split(new[] { ',' })
                    .Select(a => StringWithQualityHeaderValue.Parse(a))
                    .Select(a => new StringWithQualityHeaderValue(a.Value,
                        a.Quality.GetValueOrDefault(1)))
                    .OrderByDescending(a => a.Quality)
                    .Select(x => CultureInfo.GetCultureInfo(x.Value))
                    .FirstOrDefault();

                if (lang != null)
                {
                    Thread.CurrentThread.CurrentUICulture = lang;
                    Thread.CurrentThread.CurrentCulture = lang;
                }
            }

            if(context.Hub.Context.User == null)
                throw new HubException(Text.MainHub_SessionError);
            var user = context.Hub.Context.User.Identity as User;
            if (user == null)
                throw new HubException(Text.MainHub_SessionError);

            return base.OnBeforeIncoming(context);
        }
    } 
}