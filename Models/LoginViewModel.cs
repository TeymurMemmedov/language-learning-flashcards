using System.ComponentModel.DataAnnotations;

namespace LearningCards.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email yaxud istifadəçi adını daxil edin")]

        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._]*$|^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage ="İstifadəçi adı və ya email formatı doğru deyil")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Şifrə boş qoyula bilməz")]
        public string Password { get; set; }
    }
}
