using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
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

        // POST: api/recipes
        [Authorize] // يجب تسجيل الدخول
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Recipe recipe)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // هنا يمكن تضيفي UserId من الـ Token لو حابة
            await _recipeService.AddRecipeAsync(recipe);
            return CreatedAtAction(nameof(GetById), new { id = recipe.RecipeId }, recipe);
        }

        // PUT: api/recipes/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Recipe recipe)
        {
            if (id != recipe.RecipeId)
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
    }
}