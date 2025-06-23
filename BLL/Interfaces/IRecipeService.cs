using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task AddRecipeAsync(Recipe recipe);
        Task UpdateRecipeAsync(Recipe recipe);
        Task DeleteRecipeAsync(int id);
        Task<IEnumerable<Recipe>> SearchRecipesAsync(string term);
        Task<bool> RateRecipeAsync(int recipeId, string userId, int value);




    }
}
