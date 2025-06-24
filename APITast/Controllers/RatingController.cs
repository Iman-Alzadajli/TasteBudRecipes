using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using System.Security.Claims;

namespace APITast.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // POST: api/rating/{recipeId}
        // user can do rate 
        [Authorize]
        [HttpPost("{recipeId}")]
        public async Task<IActionResult> AddRating(int recipeId, [FromBody] int ratingValue)
        {
            if (ratingValue < 1 || ratingValue > 5)
                return BadRequest("Rating must be between 1 and 5");

            // user token 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // add rating 
            var success = await _ratingService.RateRecipeAsync(recipeId, userId, ratingValue);
            if (!success)
                return BadRequest("Failed to add rating");

            return Ok("Rating added successfully");
        }

        //  GET: api/rating/{recipeId}
        // all rating in spesifc recipe 
        [HttpGet("{recipeId}")]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatingsForRecipe(int recipeId)
        {
            var ratings = await _ratingService.GetRatingsByRecipeIdAsync(recipeId);
            return Ok(ratings);
        }
    }
}