﻿using System.ComponentModel.DataAnnotations;

namespace LearningCards.Models
{
    public class SetViewModel


    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Başlıq boş qoyula bilməz")]
        [StringLength(100, ErrorMessage = "Başlıq 100 simvoldan uzun ola bilməz")]
        public string Title { get; set; }

        public string? Description { get; set; } = "";

        public bool isPublic { get; set; }

        public string? ownerUserName { get; set; }
        
        public List<TermViewModel> TermsViewModels { get;set; } = new List<TermViewModel>() { 

            
        };



    }
}
