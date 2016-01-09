using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;

namespace Octgn.UI
{
    internal class LoggerOwinMiddleware : OwinMiddleware
    {
        private readonly ILogger _logger;
        public LoggerOwinMiddleware(OwinMiddleware next, IAppBuilder app): base (next)
        {
            _logger = app.CreateLogger<LoggerOwinMiddleware>();
        }

        public async override Task Invoke(IOwinContext context)
        {
            _logger.WriteVerbose(string.Format("{0} {1}: {2}", context.Request.Scheme, context.Request.Method, context.Request.Path));
            await Next.Invoke(context);
        }
    }
}