using Nancy;
using Octgn.Client.Models.Games;

namespace Octgn.Client.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(UIBackend uiBackend)
        {
            Get["/"] = x =>
            {
                return View["Index"];
            };
        }
    }

    public class GamesModule : NancyModule
    {
        public GamesModule(UIBackend uiBackend) : base("/Games")
        {
            Get["/{id}"] = ctx => {
                var id = (int)ctx.id;
                if (!uiBackend.GameExists(id)) return HttpStatusCode.NotFound;

                return View[new TableModel() { Id = id}];
            };
        }
    }
}