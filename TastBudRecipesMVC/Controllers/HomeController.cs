using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.ViewModels; // تأكد هذا موجود
using System.Net.Http.Json;

namespace TastBudRecipesMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        //We add categoryId to filter by classification when the user clicks the category button
        public async Task<IActionResult> Index(string term = "", int? categoryId = null)
        {
            var categories = new List<Category>();
            var recipes = new List<RecipeInputVM>();

            // Get categories
            var catResponse = await _httpClient.GetAsync("https://localhost:7218/api/category");
            if (catResponse.IsSuccessStatusCode)
            {
                categories = await catResponse.Content.ReadFromJsonAsync<List<Category>>();
            }

            //Select the appropriate link based on search or category
            string recipeEndpoint;

            if (!string.IsNullOrWhiteSpace(term))
            {
                recipeEndpoint = $"https://localhost:7218/api/recipes/search?term={term}";
            }
            else if (categoryId != null) // if category not null 
            {
                recipeEndpoint = $"https://localhost:7218/api/recipes/bycategory/{categoryId}";
            }
            else //if no search by name or category 
            {
                recipeEndpoint = "https://localhost:7218/api/recipes";
            }

            // Get recipes with search or category filter if provided
            var recipeResponse = await _httpClient.GetAsync(recipeEndpoint);
            if (recipeResponse.IsSuccessStatusCode)
            {
                var recipeEntities = await recipeResponse.Content.ReadFromJsonAsync<List<Recipe>>();
                recipes = recipeEntities.Select(r => new RecipeInputVM
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    Ingredients = r.Ingredients,
                    Instructions = r.Instructions,
                    PrepTimeMinutes = r.PrepTimeMinutes,
                    CookTimeMinutes = r.CookTimeMinutes,
                    Servings = r.Servings,
                    Difficulty = r.Difficulty,
                    CategoryId = r.CategoryId
                }).ToList();
            }

            ViewBag.SearchTerm = term; //save tem in view bag then use in in the input in the chtml 

            var vm = new HomePageVM //this is view model which have this things in it 
            {
                Categories = categories,
                Recipes = recipes
            };

            return View(vm); // this will go to frontend 
        }
    }
}