using System.ComponentModel.DataAnnotations;

namespace LearningCards.Models
{
    public class TermViewModel
    {

        [Required(ErrorMessage = "Bu hissə boş qoyula bilməz")]
        public string Term { get; set; } = "...";


        [Display(Name = "Term Definition")]

        public string TermDefinition { get; set; } = "...";
    }
}
