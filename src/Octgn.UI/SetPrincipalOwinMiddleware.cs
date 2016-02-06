using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;
using Ninject;
using System.Security.Principal;

namespace Octgn.UI
{
    internal class SetPrincipalOwinMiddleware : OwinMiddleware
    {
        private readonly ILogger _logger;
        private readonly IKernel _kernel;
        public SetPrincipalOwinMiddleware(OwinMiddleware next, IAppBuilder app, IKernel kernel): base (next)
        {
            //_logger = app.CreateLogger<LoggerOwinMiddleware>();
            _kernel = kernel;
        }

        public async override Task Invoke(IOwinContext context)
        {
            var sid = context.Request.Query["sid"];
            if (sid == null)
            {
                context.Request.User = null;
                await Next.Invoke(context);
                return;
            }

            var s = _kernel.Get<UserSessions>();
            var user = s.Get(sid);
            if (user == null)
            {
                context.Request.User = null;
                await Next.Invoke(context);
                return;
            }
			if (context.Request.Path.ToString().StartsWith("/Games/"))
			{
				var snum = context.Request.Path.ToString().Split(new[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries)[1];
				var num = int.Parse(snum);
				user.CurrentGameClient = user.GetGame(num);
			}
            context.Request.User = new GenericPrincipal(user, new string[]{});
            await Next.Invoke(context);
        }
    }
}