using System.ComponentModel.DataAnnotations;

namespace Octgn.UI.Models.Home
{
    public class LoginModel
    {
        [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9_]+$",ErrorMessage = "*LOCALIZE THIS* Invalid Username")]
        public string Username { get; set; }

        public string Token { get; set; }
    }
}