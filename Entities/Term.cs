using System.ComponentModel.DataAnnotations;

namespace LearningCards.Entities
{
    public class Term
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Definition { get; set; }

        public TermSet TermSet { get; set; }

        public int TermSetId { get; set; }
    }
}
