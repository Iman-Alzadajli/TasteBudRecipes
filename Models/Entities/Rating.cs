using Models.Entities.UserMangment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;


namespace Models.Entities
{
    public class Rating : BaseModel
    {

        public int Score { get; set; } // 1 to 5

        public string Comment { get; set; }

        public DateTime RatedAt { get; set; } = DateTime.UtcNow;

        //fk and Navigation Properties
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
