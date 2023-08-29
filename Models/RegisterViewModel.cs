using System.ComponentModel.DataAnnotations;

namespace LearningCards.Models
{
    public class RegisterViewModel
    {


        [Required(ErrorMessage ="Email boş qoyula bilməz")]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }


        [Required(ErrorMessage="İstifadəçi adı boş qoyula bilməz")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "İstifadəçi adı minimum 5, maksimum 30 simvoldan ibarət olmalıdır")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._]*$", ErrorMessage = "Invalid username format.")]
        public string UserName { get; set; }


        [Required(ErrorMessage ="Şifrə boş qoyula bilməz")]
        [StringLength(30, MinimumLength =8, ErrorMessage = "Şifrə minimum 8 simvoldan ibarət olmalıdır")]
        public string Password { get; set; }

        [Required(ErrorMessage ="Şifrənizi təsdiqləmək üçün yenidən daxil edin")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Şifrə minimum 8 simvoldan ibarət olmalıdır")]

        [Compare(nameof(Password),ErrorMessage ="Daxil etdiyiniz şifrələr eyni deyil")]
        public string RePassword { get; set; }


        [Required]
        [RegularExpression("Student")]
        public string RoleName { get; set; } = "Student";
    }
}
