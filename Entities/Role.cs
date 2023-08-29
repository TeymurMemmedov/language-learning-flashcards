using System.ComponentModel.DataAnnotations;

namespace LearningCards.Entities
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 5)]
        public string Name { get; set; }


        public string Description { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
