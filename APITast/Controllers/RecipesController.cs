using BLL.Interfaces;     
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Entities.Enums;
using Models.ViewModels;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        // GET: api/recipes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            return Ok(recipes);
        }

        // GET: api/recipes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
                return NotFound("Recipe not found");

            return Ok(recipe);
        }

        // GET: api/recipes/search?term=chicken
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var results = await _recipeService.SearchRecipesAsync(term);
            return Ok(results);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RecipeInputVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var recipe = new Recipe
            {
                Title = model.Title,
                Description = model.Description,
                Ingredients = model.Ingredients,
                Instructions = model.Instructions,
                PrepTimeMinutes = model.PrepTimeMinutes ?? 0,
                CookTimeMinutes = model.CookTimeMinutes ?? 0,
                Servings = model.Servings ?? 0,
                Difficulty = model.Difficulty ?? DifficultyLevel.Easy,
                CategoryId = model.CategoryId,
                UserId = userId
            };

            await _recipeService.AddRecipeAsync(recipe);
            return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
        }



        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RecipeInputVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Id == null || model.Id != id)
                return BadRequest("Recipe ID mismatch");

            var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);
            if (existingRecipe == null)
                return NotFound("Recipe not found");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && existingRecipe.UserId != userId)
                return Forbid("You are not allowed to edit this recipe");

            // Update fields
            existingRecipe.Title = model.Title;
            existingRecipe.Description = model.Description;
            existingRecipe.Ingredients = model.Ingredients;
            existingRecipe.Instructions = model.Instructions;
            existingRecipe.PrepTimeMinutes = model.PrepTimeMinutes ?? 0;
            existingRecipe.CookTimeMinutes = model.CookTimeMinutes ?? 0;
            existingRecipe.Servings = model.Servings ?? 0;
            existingRecipe.Difficulty = model.Difficulty ?? DifficultyLevel.Easy;
            existingRecipe.CategoryId = model.CategoryId;

            await _recipeService.UpdateRecipeAsync(existingRecipe);
            return NoContent();
        }


        // DELETE: api/recipes/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // if its avalible before able to delete 
            var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);
            if (existingRecipe == null)
                return NotFound("Recipe not found");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // admin and user can delete it 
            if (!isAdmin && existingRecipe.UserId != userId)
                return Forbid("You are not allowed to delete this recipe");

            await _recipeService.DeleteRecipeAsync(id);
            return NoContent();
        }

        // POST: api/recipes/{id}/rate
        [Authorize]
        [HttpPost("{id}/rate")]
        public async Task<IActionResult> Rate(int id, [FromBody] int value)
        {
            if (value < 1 || value > 5)
                return BadRequest("Rating must be between 1 and 5");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // check if it avalible before be able to rate 
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
                return NotFound("Recipe not found");

            var success = await _recipeService.RateRecipeAsync(id, userId, value);
            if (!success)
                return BadRequest("Unable to rate recipe");

            return Ok("Recipe rated successfully");
        }



        // GET: api/recipes/bycategory/5
        [HttpGet("bycategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var recipes = await _recipeService.GetRecipesByCategoryAsync(categoryId);
            if (recipes == null || !recipes.Any())
                return NotFound("No recipes found for this category.");

            return Ok(recipes);
        }

        // get rec using userid

        // GET: api/recipes/myrecipes
        [Authorize]
        [HttpGet("myrecipes")]
        public async Task<IActionResult> GetMyRecipes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            var recipes = await _recipeService.GetRecipesByUserIdAsync(userId);
            if (recipes == null || !recipes.Any())
            {
                return NotFound("No recipes found for this user.");
            }

            return Ok(recipes);
        }


        // أضف هذا في RecipesController.cs (API)

        // GET: api/recipes/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRecipesByUserId(string userId)
        {
            Console.WriteLine($"API: Looking for recipes for userId: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var recipes = await _recipeService.GetRecipesByUserIdAsync(userId);

            Console.WriteLine($"API: Found {recipes?.Count() ?? 0} recipes for user {userId}");

            if (recipes == null || !recipes.Any())
            {
                return NotFound("No recipes found for this user.");
            }

            return Ok(recipes);
        }






    }
}