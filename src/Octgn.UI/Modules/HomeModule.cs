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
            Get["/{id}"] = ctx =>
            {
                var id = (int)ctx.id;
				var user = (this.Context.CurrentUser as User);
				var game = user.GetGame(id);
				if (game == null) return HttpStatusCode.NotFound;

                return View[new TableModel(game)];
            };

            Get["/Resources/"] = ctx =>
            {
                return null;
            };
        }
    }
}


