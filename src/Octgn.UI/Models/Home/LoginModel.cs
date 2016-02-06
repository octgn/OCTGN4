using Octgn.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Octgn.UI.Models.Home
{
    public class LoginModel
    {
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Models_Home_LoginModel_UsernameRequired")]
        [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9\-_ ]+$", ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Models_Home_LoginModel_UsernameInvalid")]
        public string Username { get; set; }

        public string Token { get; set; }
    }
}