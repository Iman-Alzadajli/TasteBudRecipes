using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;

namespace TastBudRecipesMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }


        //register 

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7218/");

            var response = await httpClient.PostAsJsonAsync("api/Auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

           
            var errorResponse = await response.Content.ReadAsStringAsync();

            try
            {
                var errors = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Microsoft.AspNetCore.Identity.IdentityError>>(errorResponse);
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);// just to check for error 
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Registration failed: " + errorResponse);
            }

            return View(model);
        }




        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7218/");

            var response = await httpClient.PostAsJsonAsync("api/Auth/login", model);

            if (response.IsSuccessStatusCode)
            {
             
                var token = await response.Content.ReadAsStringAsync();

               
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Login failed.");
            return View(model);
        }
    }
}