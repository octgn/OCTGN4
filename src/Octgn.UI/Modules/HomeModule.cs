using Nancy;
using Nancy.ModelBinding;
using Octgn.UI.Models.Games;
using Octgn.UI.Models.Home;
using System.Text.RegularExpressions;

namespace Octgn.UI.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(UserSessions sessions)
        {
            Get["/"] = x =>
            {
                return View["Index"];
            };

            Get["/Login"] = _ => View["Login", new LoginModel()];

            Post["/Login"] = data =>
            {
                var login = this.Bind<LoginModel>();
                if(!Regex.IsMatch(login.Username, @"^[a-zA-Z0-9]+[a-zA-Z0-9_]+$"))
                {
                    this.ModelValidationResult.Errors.Add("Username", "*LOCALIZE THIS* Username can't be empty");
                    return View["Login", login];
                }

                var user = sessions.Create(login.Username);
                return Response.AsRedirect("/?sid=" + user.Sid);
            };

        }
    }

    public class GamesModule : NancyModule
    {
        public GamesModule(LocalServerManager locserver) : base("/Games")
        {
            Get["/{id}/"] = ctx =>
            {
                var id = (int)ctx.id;
				var user = (this.Context.CurrentUser as User);
				var game = user.GetGame(id);
				if (game == null) return HttpStatusCode.NotFound;

                return View[new TableModel(game)];
            };

            Get["/{id}/Resources/{pars*}"] = ctx =>
            {
				var user = (this.Context.CurrentUser as User);
				Gameplay.ResourceResolver.ResourceResolverResult resource = user.GameClient.ResourceResolver.Get(ctx.pars);

				if (resource.StatusCode == 404)
					return HttpStatusCode.NotFound;
				else if (resource.StatusCode == 408)
					return HttpStatusCode.RequestTimeout;

				if (resource.ContentType.StartsWith("text"))
				{
					var strData = System.Text.Encoding.UTF8.GetString(resource.Data);
					const string rex = "('|\")(Resources/[a-zA-Z\\.]+){1}('|\")";

					var newData = Regex.Replace(strData, rex, $"$1$2?sid={user.Sid}$3");
                    resource.Data = System.Text.Encoding.UTF8.GetBytes(newData);
				}

				var r = new Response();
				r.Contents = s =>
				{
					s.Write(resource.Data, 0, resource.Data.Length);
				};
				r.ContentType = resource.ContentType;
				return r;
            };
        }
    }
}


