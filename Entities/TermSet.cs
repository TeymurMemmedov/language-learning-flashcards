using Microsoft.Extensions.Configuration.UserSecrets;
using System.ComponentModel.DataAnnotations;

namespace LearningCards.Entities
{
    public class TermSet
    {
        public TermSet() { 
            Terms= new List<Term>();
        }
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Başlıq 100 simvoldan qısa olmalıdır")]

        public string Title { get; set; }

        public string? Description { get; set; }
        public bool isPublic { get; set; } = true;

        public User ownerUser { get; set; }
        public int ownerUserId { get; set; }

        public ICollection<Term> Terms { get; set; }
    }
}
