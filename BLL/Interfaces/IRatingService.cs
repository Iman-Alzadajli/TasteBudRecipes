using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IRatingService
    {
        Task<IEnumerable<Rating>> GetAllRatingsAsync();
        Task<Rating> GetRatingByIdAsync(int id);
        Task AddRatingAsync(Rating rating);
        Task DeleteRatingAsync(int id);
        Task<bool> RateRecipeAsync(int recipeId, string userId, int score);
        Task<IEnumerable<Rating>> GetRatingsByRecipeIdAsync(int recipeId); // bringrating  for specific recipe 


    }
}
