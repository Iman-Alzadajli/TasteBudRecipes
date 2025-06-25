using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class HomePageVM
    {
        public List<Category> Categories { get; set; }
        public List<RecipeInputVM> Recipes { get; set; }
    }
}
