using Models.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class RecipeInputVM
    {
        public int? Id { get; set; } // ID optional for Create, required for Update

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Ingredients { get; set; }

        public string Instructions { get; set; }

        public int? PrepTimeMinutes { get; set; }

        public int? CookTimeMinutes { get; set; }

        public int? Servings { get; set; }

        public DifficultyLevel? Difficulty { get; set; }

        [Required(ErrorMessage = "CategoryId is required.")]
        public int CategoryId { get; set; }
    }
}