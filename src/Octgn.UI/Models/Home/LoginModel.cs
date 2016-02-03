using Octgn.UI.Resources;
using System.ComponentModel.DataAnnotations;

namespace Octgn.UI.Models.Home
{
    public class LoginModel
    {
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Models_Home_LoginModel_Username_Required")]
        [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9_]+$", ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Models_Home_LoginModel_Username_Invalid"]
        [StringLength(24, MinimumLength = 2, ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Models_Home_LoginModel_Username_WrongLength")]
        public string Username { get; set; }

        public string Token { get; set; }
    }
}