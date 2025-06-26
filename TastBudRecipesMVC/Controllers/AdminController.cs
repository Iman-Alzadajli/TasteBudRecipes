using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using System.Net.Http;

namespace TastBudRecipesMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: /Admin/AllRecipes
        public async Task<IActionResult> AllRecipes()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7218/api/recipes");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var allRecipes = await response.Content.ReadFromJsonAsync<List<Recipe>>();


                // category name 

                
                if (allRecipes != null)
                {
                    foreach (var recipe in allRecipes)
                    {
                        if (recipe.Category == null && recipe.CategoryId != 0)
                        {
                            var categoryResponse = await _httpClient.GetAsync($"https://localhost:7218/api/category/{recipe.CategoryId}");
                            if (categoryResponse.IsSuccessStatusCode)
                            {
                                recipe.Category = await categoryResponse.Content.ReadFromJsonAsync<Category>();
                            }
                        }
                    }
                }

                // the end 
                return View(allRecipes);
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Failed to load recipes: {error}");
            return View(new List<Recipe>());
        }

        // POST: /Admin/DeleteRecipe
        [HttpPost]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7218/api/recipes/{id}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Recipe deleted successfully!";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"❌ Failed to delete recipe: {error}";
            }

            return RedirectToAction("AllRecipes");
        }
    }
}
