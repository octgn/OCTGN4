using Microsoft.AspNet.Mvc;
using Octgn.Shared;
using Octgn.UI.Models.Games;
using Octgn.UI.Resources;
using System.Text.RegularExpressions;

namespace Octgn.UI.Controllers
{
    public class GamesController : Controller
    {
        protected ILogger Log = LoggerFactory.Create<GamesController>();

        private LocalServerManager _locserver;
        public GamesController(LocalServerManager locserver)
        {
            _locserver = locserver;
        }

        [HttpPut]
        public IActionResult Index(HostGameModel game)
        {
            try
            {
                ViewBag.Title = "Game Table";
                if (!this.ModelState.IsValid)
                {
                    this.Response.StatusCode = 400;
                    return Json(this.ModelState);
                }
                var user = this.User.Identity as User;
                var server = _locserver.LaunchServer(game.GameName);
                var client = user.JoinGame("localhost:" + server.Port);
                if (!client.Connect())
                {
                    this.Response.StatusCode = 500;
                    return this.Content(Text.MainHub_HostGame_UnhandledError);
                }
                return Content(client.Id.ToString());
            }
            catch (System.Exception e)
            {
                Log.Error(e.ToString());
                this.Response.StatusCode = 500;
                return Content(Text.MainHub_HostGame_UnhandledError);
            }
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            var user = this.User.Identity as User;
            var game = user.GetGame(id);
            if (game == null) return HttpNotFound();

            return View(new TableModel(game));
        }

        [HttpGet]
        public IActionResult Resources(string path)
        {
            var user = this.User.Identity as User;
            Gameplay.ResourceResolver.ResourceResolverResult resource = user.GameClient.ResourceResolver.Get(path);

            if (resource.StatusCode == 404)
                return HttpNotFound();
            else if (resource.StatusCode == 408)
                return new HttpStatusCodeResult(408);

            if (resource.ContentType.StartsWith("text"))
            {
                var strData = System.Text.Encoding.UTF8.GetString(resource.Data);
                const string rex = "('|\")(Resources/[a-zA-Z\\.]+){1}('|\")";

                var newData = Regex.Replace(strData, rex, $"$1$2?sid={user.Sid}$3");
                resource.Data = System.Text.Encoding.UTF8.GetBytes(newData);
            }

            return File(resource.Data, resource.ContentType);
        }
    }
}
