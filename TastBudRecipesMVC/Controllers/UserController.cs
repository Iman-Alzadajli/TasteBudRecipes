using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.ViewModels;

[Authorize(Roles = "User")]
public class UserController : Controller
{
    private readonly HttpClient _httpClient;

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
        // *** START OF MODIFICATION FOR MyRecipes GET ***
        var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7218/api/recipes/myrecipes");
        if (Request.Headers.ContainsKey("Cookie"))
        {
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());
        }

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var myRecipes = await response.Content.ReadFromJsonAsync<List<Recipe>>();
            return View(myRecipes);
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound) // No recipes found for user
        {
            ViewBag.Message = "You haven't created any recipes yet.";
            return View(new List<Recipe>()); // Return an empty list to the view
        }
        else
        {
            // Handle other API errors
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Failed to load your recipes: {error}");
            return View(new List<Recipe>()); // Return empty list on error
        }
        // *** END OF MODIFICATION FOR MyRecipes GET ***
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

        var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7218/api/recipes");
        request.Content = JsonContent.Create(model);

        if (Request.Headers.ContainsKey("Cookie"))
        {
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());
        }

        var response = await _httpClient.SendAsync(request);

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
    [HttpGet]
    public async Task<IActionResult> EditRecipe(int id)
    {
        // *** START OF MODIFICATION FOR EditRecipe GET ***
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7218/api/recipes/{id}");
        if (Request.Headers.ContainsKey("Cookie"))
        {
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());
        }

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return NotFound("Recipe not found or you don't have permission to edit it.");
        }

        var recipe = await response.Content.ReadFromJsonAsync<Recipe>();

        // Fetch categories for the dropdown
        var categoriesResponse = await _httpClient.GetAsync("https://localhost:7218/api/category");
        var categories = new List<Category>();
        if (categoriesResponse.IsSuccessStatusCode)
        {
            categories = await categoriesResponse.Content.ReadFromJsonAsync<List<Category>>();
        }
        ViewBag.Categories = categories
            .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
            .ToList();


        //Fetch category data based on CategoryId
        var categoryResponse = await _httpClient.GetAsync($"https://localhost:7218/api/category/{recipe.CategoryId}");
        Category category = null;
        if (categoryResponse.IsSuccessStatusCode)
        {
            category = await categoryResponse.Content.ReadFromJsonAsync<Category>();
        }


        var model = new RecipeInputVM
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

        return View(model);
       
    }

    // POST: /User/EditRecipe
    // Save the updated recipe details
    [HttpPost]
    public async Task<IActionResult> EditRecipe(RecipeInputVM model)
    {
      
        // Check if form is valid
        if (!ModelState.IsValid)
        {
            //fetch categories if validation fails to repopulate dropdown
            var categoriesResponse = await _httpClient.GetAsync("https://localhost:7218/api/category");
            var categories = new List<Category>();
            if (categoriesResponse.IsSuccessStatusCode)
            {
                categories = await categoriesResponse.Content.ReadFromJsonAsync<List<Category>>();
            }
            ViewBag.Categories = categories
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
                .ToList();
            return View(model); // If invalid show the form again
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7218/api/recipes/{model.Id}");
        request.Content = JsonContent.Create(model);

        if (Request.Headers.ContainsKey("Cookie"))
        {
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());
        }

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Recipe updated successfully!"; 
            return RedirectToAction("MyRecipes"); // After update return to user recipe list
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"❌ Failed to update recipe: {error}");

            // Re-fetch categories if API update fails to repopulate dropdown
            var categoriesResponse = await _httpClient.GetAsync("https://localhost:7218/api/category");
            var categories = new List<Category>();
            if (categoriesResponse.IsSuccessStatusCode)
            {
                categories = await categoriesResponse.Content.ReadFromJsonAsync<List<Category>>();
            }
            ViewBag.Categories = categories
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
                .ToList();
            return View(model);
        }
        // *** END OF MODIFICATION FOR EditRecipe POST ***
    }





    // POST: /User/DeleteRecipe/{id}
    [HttpPost]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
      
        var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7218/api/recipes/{id}");
        if (Request.Headers.ContainsKey("Cookie"))
        {
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());
        }

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

        return RedirectToAction("MyRecipes");
       
    }
}
