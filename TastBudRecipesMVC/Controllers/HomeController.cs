using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using System.Diagnostics;
using TastBudRecipesMVC.Models;

namespace TastBudRecipesMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            var categories = new List<Category>();

            var response = await _httpClient.GetAsync("https://localhost:7218/api/category");

            if (response.IsSuccessStatusCode)
            {
                categories = await response.Content.ReadFromJsonAsync<List<Category>>();
            }

            return View(categories);
        }
    }
}