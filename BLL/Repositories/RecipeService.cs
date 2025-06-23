using BLL.Interfaces;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repositories
{
    public class RecipeService : IRecipeService
    {
        private readonly IGenericRepository<Recipe> _recipeRepo;
        private readonly IGenericRepository<Rating> _ratingRepo;


        public RecipeService(IGenericRepository<Recipe> recipeRepo)
        {
            _recipeRepo = recipeRepo;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            return await _recipeRepo.GetAll();
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            return await _recipeRepo.GetById(id);
        }

        public async Task AddRecipeAsync(Recipe recipe)
        {
            await _recipeRepo.Add(recipe);
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            _recipeRepo.Update(recipe);
        }

        public async Task DeleteRecipeAsync(int id)
        {
            var recipe = await _recipeRepo.GetById(id);
            if (recipe != null)
                _recipeRepo.Delete(recipe);
        }

        public async Task<IEnumerable<Recipe>> SearchRecipesAsync(string term)
        {
            var allRecipes = await _recipeRepo.GetAll();
            return allRecipes.Where(r =>
                r.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                r.Ingredients.Contains(term, StringComparison.OrdinalIgnoreCase));
        }


        public async Task<bool> RateRecipeAsync(int recipeId, string userId, int score)
        {
            var recipe = await _recipeRepo.GetById(recipeId);
            if (recipe == null)
                return false;

            var allRatings = await _ratingRepo.GetAll();
            var existing = allRatings.FirstOrDefault(r =>
                r.RecipeId == recipeId && r.UserId == userId);

            if (existing != null)
            {
                existing.Score = score;
                _ratingRepo.Update(existing);
            }
            else
            {
                await _ratingRepo.Add(new Rating
                {
                    RecipeId = recipeId,
                    UserId = userId,
                    Score = score
                });
            }

            return true;
        }
    }
}