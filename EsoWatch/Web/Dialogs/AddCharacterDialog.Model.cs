using System.ComponentModel.DataAnnotations;

namespace EsoWatch.Web.Dialogs;

public partial class AddCharacterDialog
{
    public class Model
    {
        [Required(ErrorMessage = "Character name is required")]
        [MinLength(5, ErrorMessage = "Character name must be at least 5 characters long")]
        public string Name { get; set; } = "";
    }
}