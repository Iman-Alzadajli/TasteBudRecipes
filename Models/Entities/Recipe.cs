using Models.Entities.Enums;
using Models.Entities.UserMangment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Entities;

namespace Models.Entities
{
    public class Recipe : BaseModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }


        public int PrepTimeMinutes { get; set; }
        public int CookTimeMinutes { get; set; }
        public int Servings { get; set; }

        public DifficultyLevel Difficulty { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        //fk and Navigation Properties
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Rating> Ratings { get; set; }
    }
}
