using BLL.Interfaces;     
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;       
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        //for Dependency Injection 
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
                return NotFound();
            return Ok(recipe);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string term)
        {
            var results = await _recipeService.SearchRecipesAsync(term);
            return Ok(results);
        }



        // POST: api/recipes
        [Authorize] // for security 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Recipe recipe)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _recipeService.AddRecipeAsync(recipe);
            return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
        }

        // PUT: api/recipes/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Recipe recipe)
        {
            if (id != recipe.Id
)
                return BadRequest("Recipe ID mismatch");

            var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);
            if (existingRecipe == null)
                return NotFound();

            await _recipeService.UpdateRecipeAsync(recipe);
            return NoContent();
        }

        // DELETE: api/recipes/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);
            if (existingRecipe == null)
                return NotFound();

            await _recipeService.DeleteRecipeAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/rate")]
        public async Task<IActionResult> Rate(int id, [FromBody] int value)
        {
            if (value < 1 || value > 5)
                return BadRequest("Rating must be between 1 and 5");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var success = await _recipeService.RateRecipeAsync(id, userId, value);
            if (!success)
                return BadRequest("Unable to rate recipe.");

            return Ok("Recipe rated successfully");
        }
    }
}



    

