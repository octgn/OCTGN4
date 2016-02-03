using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Principal;
using System.Security.Claims;

namespace Octgn.UI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var user = GetUser(context);
            if(user != null)
            {
                context.User = user;
                
                await _next(context);
                return;
            }
            context.User = null;

            if(context.Request.Path.HasValue == false)
            {
                //await _next(context);
                context.Response.Redirect("/Home/Login");
                return;
            }
            if (context.Request.Path.Value.ToLower().StartsWith("/home/login") == false)
            {
                //await _next(context);
                context.Response.Redirect("/Home/Login");
                return;
            }
            await _next(context);
        }

        private ClaimsPrincipal GetUser(HttpContext context)
        {
            if (context.Request.Query.ContainsKey("sid") == false)
            {
                return null;
            }
            var sid = context.Request.Query["sid"][0];

            var s = context.ApplicationServices.GetService<UserSessions>();
            var user = s.Get(sid);
            if (user == null)
            {
                return null;
            }
            if (context.Request.Path.ToString().StartsWith("/Games/"))
            {
                var snum = context.Request.Path.ToString().Split(new[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries)[1];
                var num = int.Parse(snum);
                user.GameClient = user.GetGame(num);
            }
            var ret = new GenericPrincipal(user, new string[] { });
            return ret;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            var ret = builder.UseMiddleware<AuthenticationMiddleware>();
            return ret;
        }
    }
}
