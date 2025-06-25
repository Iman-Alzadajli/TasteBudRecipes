using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.ViewModels;
using System.Security.Claims;

namespace TastBudRecipesMVC.Controllers
{
    public class RecipesController : Controller
    {
        private readonly HttpClient _httpClient;

        //Use IHttpClientFactory instead of direct service injection
        public RecipesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: /Recipes/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            // Fetch recipe data from API 
            var recipeResponse = await _httpClient.GetAsync($"https://localhost:7218/api/recipes/{id}");
            if (!recipeResponse.IsSuccessStatusCode)
                return NotFound("Recipe not found");

            var recipe = await recipeResponse.Content.ReadFromJsonAsync<Recipe>();

            //Fetch category data based on CategoryId
            var categoryResponse = await _httpClient.GetAsync($"https://localhost:7218/api/category/{recipe.CategoryId}");
            Category category = null;
            if (categoryResponse.IsSuccessStatusCode)
            {
                category = await categoryResponse.Content.ReadFromJsonAsync<Category>();
            }

            //  ViewModel
            var vm = new RecipeInputVM
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients,
                Instructions = recipe.Instructions,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                CookTimeMinutes = recipe.CookTimeMinutes,
                Servings = recipe.Servings,
                Difficulty = recipe.Difficulty,
                CategoryName = category?.Name ?? "Unknown"
            };

            return View(vm);
        }
    }
}