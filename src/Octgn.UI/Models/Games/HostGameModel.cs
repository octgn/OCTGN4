using Octgn.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Octgn.UI.Models.Games
{
    public class HostGameModel
    {
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "MainHub_GameNameInvalid")]
        [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9\-_ ]+$", ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "MainHub_GameNameInvalid")]
        public string GameName { get; set; }
    }

    public class JoinGameModel
    {
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Models_Games_JoinGameModel_HostRequired")]
        public string Host { get; set; }
    }
}