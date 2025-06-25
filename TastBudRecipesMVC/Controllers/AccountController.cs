using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using System.Security.Claims;

namespace TastBudRecipesMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient; 

        public AccountController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient(); //send request mvc to api 
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

            var httpClient = new HttpClient(); //create object and this will be the api 
            httpClient.BaseAddress = new Uri("https://localhost:7218/");

            var response = await httpClient.PostAsJsonAsync("api/Auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

           
            var errorResponse = await response.Content.ReadAsStringAsync();//give message 

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




        //login 


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

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Login failed.");
                return View(model);
            }

            // get role tho the api 
            var roleResponse = await httpClient.GetAsync($"api/Auth/GetRoleByEmail?email={model.Email}");
            if (!roleResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to retrieve user role.");
                return View(model);
            }

            var role = await roleResponse.Content.ReadAsStringAsync();
            role = role.Trim('"'); 

            // create  Claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, model.Email),
        new Claim(ClaimTypes.Role, role) , // this to show dashbored 
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            return RedirectToAction("Index", "Home");
        }


        // end of login 


        //logout 

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }



    }
}