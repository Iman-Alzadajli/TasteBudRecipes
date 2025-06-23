using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;


namespace Models.Entities.UserMangment
{
    public class ApplicationUser : IdentityUser
    {
       
        public string FullName { get; set; }

        // Navigation
        public ICollection<Recipe> Recipes { get; set; }
        public ICollection<Rating> Ratings { get; set; }
    }
}
