using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LearningCards.Entities
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique =true)]
    public class User
    {
        public int Id { get; set; }


        //public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._]*$")]
        [StringLength(30)]
        public string Username { get; set; }


        [Required] 
        public string Password { get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Role role { get; set; } = new Role();

        [Required]
        public int roleId { get; set; }


        public ICollection<TermSet> termSets { get; set; }

    }
}
