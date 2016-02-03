using Microsoft.AspNet.Mvc;
using Octgn.UI.Models.Home;

namespace Octgn.UI.Controllers
{
    public class HomeController : Controller
    {
        private UserSessions _sessions;
        public HomeController(UserSessions sessions)
        {
            _sessions = sessions;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.Title = "Home";
            return View();
        }

        public IActionResult Login()
        {
            ViewBag.Title = "Login";
            return View(new LoginModel());
        }

        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            ViewBag.Title = "Login";
            if (!this.ModelState.IsValid)
            {
                return View(login);
            }

            var user = _sessions.Create(login.Username);
            return Redirect("/?sid=" + user.Sid);
        }
    }
}
