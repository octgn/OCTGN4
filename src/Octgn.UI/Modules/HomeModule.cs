using Nancy;
using Octgn.UI.Models.Games;

namespace Octgn.UI.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = x =>
            {
                return View["Index"];
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

                var server = locserver.GetServer(id);
                if (server == null) return HttpStatusCode.NotFound;

                return View[new TableModel() { Id = server.Id }];
            };
        }
    }
}