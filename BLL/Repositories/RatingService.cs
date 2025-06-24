using BLL.Interfaces;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace BLL.Repositories
{
    public class RatingService : IRatingService
    {
        private readonly IGenericRepository<Rating> _ratingRepo;
        private readonly IGenericRepository<Recipe> _recipeRepo;





        public RatingService(IGenericRepository<Rating> ratingRepo , IGenericRepository<Recipe> recipeRepo)
        {
            _ratingRepo = ratingRepo;
            _recipeRepo = recipeRepo;

           
        }

        public async Task<IEnumerable<Rating>> GetAllRatingsAsync()
        {
            return await _ratingRepo.GetAll();
        }

        public async Task<Rating> GetRatingByIdAsync(int id)
        {
            return await _ratingRepo.GetById(id);
        }

        public async Task AddRatingAsync(Rating rating)
        {
            await _ratingRepo.Add(rating);
            await _ratingRepo.SaveAsync();  // Save changes

        }

        public async Task DeleteRatingAsync(int id)
        {
            var rating = await _ratingRepo.GetById(id);
            if (rating != null)
                _ratingRepo.Delete(rating);
            await _ratingRepo.SaveAsync();  // Save changes


        }



        public async Task<bool> RateRecipeAsync(int recipeId, string userId, int score)
        {
            var recipe = await _recipeRepo.GetById(recipeId);
            if (recipe == null)
                return false;

            var existingRatings = await _ratingRepo.Find(r => r.RecipeId == recipeId && r.UserId == userId);
            var existing = existingRatings.FirstOrDefault();

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
                    Score = score,
                    Comment = "" // أو "No comment"
                });
            }

            await _ratingRepo.SaveAsync();
            return true;
        }



        public async Task<IEnumerable<Rating>> GetRatingsByRecipeIdAsync(int recipeId)
        {
            return await _ratingRepo.Find(r => r.RecipeId == recipeId);
        }

      



    }
}
