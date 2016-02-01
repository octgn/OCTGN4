using System.ComponentModel.DataAnnotations;
using Octgn.UI.Resources;

namespace Octgn.UI.Models.Games
{
    public class HostGameModel
    {
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "MainHub_GameNameInvalid")]
        [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9\-_ ]+$", ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "MainHub_GameNameInvalid")]
        public string GameName { get; set; }
    }
}