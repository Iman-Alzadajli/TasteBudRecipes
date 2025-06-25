using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.ViewModels;
using System.Net.Http;


namespace TastBudRecipesMVC.Controllers


    //show user dashnored 
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        //Use IHttpClientFactory instead of direct service injection
        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    


    // GET: /User/MyRecipes
        // This will show only recipes created by the own user
        public async Task<IActionResult> MyRecipes()
        {
          
            return View(); 
        }



        //  GET: /User/AddRecipe
        // Show form with category dropdown
        [HttpGet]
        public async Task<IActionResult> AddRecipe()
        {
            var categoriesResponse = await _httpClient.GetAsync("https://localhost:7218/api/category");
            var categories = new List<Category>();

            if (categoriesResponse.IsSuccessStatusCode)
            {
                categories = await categoriesResponse.Content.ReadFromJsonAsync<List<Category>>();
            }

            ViewBag.Categories = categories
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
                .ToList();

            return View(new RecipeInputVM()); // return with empty model
        }


        //  POST: /User/AddRecipe
        [HttpPost]
        public async Task<IActionResult> AddRecipe(RecipeInputVM model)
        {
            // 1. تحميل الكاتجوري للدروبداون في حال فشل الحفظ
            var categoriesResponse = await _httpClient.GetAsync("https://localhost:7218/api/category");
            var categories = new List<Category>();

            if (categoriesResponse.IsSuccessStatusCode)
            {
                categories = await categoriesResponse.Content.ReadFromJsonAsync<List<Category>>();
            }

            ViewBag.Categories = categories
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
                .ToList();

            if (!ModelState.IsValid)
                return View(model);

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7218/api/recipes", model);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Success = true;
                ModelState.Clear(); 
                return View(new RecipeInputVM()); 
            }

          
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"❌ Failed to add recipe: {error}");
            return View(model);
        }






        // GET: /User/EditRecipe/{id}
        // Load a recipe by its ID for editing
        public async Task<IActionResult> EditRecipe(int id)
        {

            return View(); // Show form with recipe data (once retrieved)
        }

        // POST: /User/EditRecipe
        // Save the updated recipe details
        [HttpPost]
        public async Task<IActionResult> EditRecipe(RecipeInputVM model)
        {
            // 1. Check if form is valid
            if (!ModelState.IsValid)
                return View(model); // If invalid  show the form again

          
            return RedirectToAction("MyRecipes"); // After update  return to user recipe list
        }

        //



       

    }
}