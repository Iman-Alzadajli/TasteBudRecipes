using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.ViewModels;
using System.Net.Http;
using System.Security.Claims;

[Authorize(Roles = "User")]
public class UserController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpClient CreateHttpClient()
    {
        var client = _httpClientFactory.CreateClient();
        var context = _httpContextAccessor.HttpContext;
        if (context != null && context.Request.Headers.TryGetValue("Cookie", out var cookieHeader))
        {
            client.DefaultRequestHeaders.Remove("Cookie");
            client.DefaultRequestHeaders.Add("Cookie", cookieHeader.ToString());
        }
        return client;
    }

    public IActionResult Dashboard()
    {
        return View();
    }




    // GET: /User/MyRecipes - مع debugging
    public async Task<IActionResult> MyRecipes()
    {
        var client = CreateHttpClient();

     
        Console.WriteLine($"User is authenticated: {User.Identity.IsAuthenticated}");
        Console.WriteLine($"User name: {User.Identity.Name}");
        Console.WriteLine($"All Claims:");

        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"  {claim.Type}: {claim.Value}");
        }

     
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine($"UserId from NameIdentifier: {userId}");

    
        var userIdAlt1 = User.FindFirstValue("sub");
        var userIdAlt2 = User.FindFirstValue("id");
        var userIdAlt3 = User.FindFirstValue("user_id");
        var userName = User.Identity.Name;

        Console.WriteLine($"Alternative userId methods:");
        Console.WriteLine($"  sub: {userIdAlt1}");
        Console.WriteLine($"  id: {userIdAlt2}");
        Console.WriteLine($"  user_id: {userIdAlt3}");
        Console.WriteLine($"  userName: {userName}");

        
        var finalUserId = userId ?? userIdAlt1 ?? userIdAlt2 ?? userIdAlt3 ?? userName;

        if (string.IsNullOrEmpty(finalUserId))
        {
            Console.WriteLine("No user identifier found - redirecting to login");
            return RedirectToAction("Login", "Account");
        }

        Console.WriteLine($"Using userId: {finalUserId}");

        
        var response = await client.GetAsync($"https://localhost:7218/api/recipes/user/{finalUserId}");

        Console.WriteLine($"API Response Status: {response.StatusCode}");

        if (response.IsSuccessStatusCode)
        {
            var myRecipes = await response.Content.ReadFromJsonAsync<List<Recipe>>();
            Console.WriteLine($"Found {myRecipes?.Count ?? 0} recipes");

            if (myRecipes != null)
            {
                foreach (var recipe in myRecipes)
                {
                    if (recipe.Category == null && recipe.CategoryId != 0)
                    {
                        var categoryResponse = await client.GetAsync($"https://localhost:7218/api/category/{recipe.CategoryId}");
                        if (categoryResponse.IsSuccessStatusCode)
                        {
                            recipe.Category = await categoryResponse.Content.ReadFromJsonAsync<Category>();
                        }
                    }
                }
            }

            return View(myRecipes ?? new List<Recipe>());
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            ViewBag.Message = "You haven't created any recipes yet.";
            return View(new List<Recipe>());
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Error: {error}");
            ModelState.AddModelError("", $"Failed to load your recipes: {error}");
            return View(new List<Recipe>());
        }
    }

    //



    // GET: /User/AddRecipe
    [HttpGet]
    public async Task<IActionResult> AddRecipe()
    {
        var client = CreateHttpClient();

        var categoriesResponse = await client.GetAsync("https://localhost:7218/api/category");
        var categories = new List<Category>();

        if (categoriesResponse.IsSuccessStatusCode)
        {
            categories = await categoriesResponse.Content.ReadFromJsonAsync<List<Category>>();
        }

        ViewBag.Categories = categories
            .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
            .ToList();

        return View(new RecipeInputVM());
    }







    // POST: /User/AddRecipe
    [HttpPost]
    public async Task<IActionResult> AddRecipe(RecipeInputVM model)
    {
        var client = CreateHttpClient();

        // 
        var categoriesResponse = await client.GetAsync("https://localhost:7218/api/category");
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

        var response = await client.PostAsJsonAsync("https://localhost:7218/api/recipes", model);

        if (response.IsSuccessStatusCode)
        {
            ViewBag.Success = true;
            ModelState.Clear();
            return View(new RecipeInputVM());
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"❌ Failed to add recipe: {error}");
            return View(model);
        }
    }






    // GET: /User/EditRecipe/{id}
    [HttpGet]
    public async Task<IActionResult> EditRecipe(int id)
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync($"https://localhost:7218/api/recipes/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound("Recipe not found or you don't have permission to edit it.");
        }

        var recipe = await response.Content.ReadFromJsonAsync<Recipe>();

        var categoriesResponse = await client.GetAsync("https://localhost:7218/api/category");
        var categories = new List<Category>();
        if (categoriesResponse.IsSuccessStatusCode)
        {
            categories = await categoriesResponse.Content.ReadFromJsonAsync<List<Category>>();
        }
        ViewBag.Categories = categories
            .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
            .ToList();

        Category category = null;
        if (recipe.CategoryId != 0)
        {
            var categoryResponse = await client.GetAsync($"https://localhost:7218/api/category/{recipe.CategoryId}");
            if (categoryResponse.IsSuccessStatusCode)
            {
                category = await categoryResponse.Content.ReadFromJsonAsync<Category>();
            }
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
            CategoryName = category?.Name ?? "Unknown",
            CategoryId = recipe.CategoryId
        };

        return View(model);
    }





    // POST: /User/EditRecipe
    [HttpPost]
    public async Task<IActionResult> EditRecipe(RecipeInputVM model)
    {
        var client = CreateHttpClient();

        // Reload categories if form validation fails
        if (!ModelState.IsValid)
        {
            var categoriesResponse = await client.GetAsync("https://localhost:7218/api/category");
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

        var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7218/api/recipes/{model.Id}")
        {
            Content = JsonContent.Create(model)
        };

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Recipe updated successfully!";
            return RedirectToAction("MyRecipes");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"❌ Failed to update recipe: {error}");

            // Reload the categories if update fails
            var categoriesResponse = await client.GetAsync("https://localhost:7218/api/category");
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
    }




    // POST: /User/DeleteRecipe/{id}
    [HttpPost]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var client = CreateHttpClient();

        var response = await client.DeleteAsync($"https://localhost:7218/api/recipes/{id}");

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