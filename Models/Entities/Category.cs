using System;
using Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Category : BaseModel
    {

        public string Name { get; set; }

        public ICollection<Recipe> Recipes { get; set; }
    }
}
