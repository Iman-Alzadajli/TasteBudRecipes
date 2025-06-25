using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities.UserMangment;
using Models.ViewModels;

namespace APITast.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model) // from registermodel 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); //check if data is right 

            var user = new ApplicationUser // create object 
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Gender = model.Gender,
                Country = model.Country
            };

            var result = await _userManager.CreateAsync(user, model.Password); //if everything right create else error 
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User"); // the user will be a normal user 

            return Ok("User registered successfully");
        }


        //for login 
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid login attempt");

            // if user account is locked 
            if (await _userManager.IsLockedOutAsync(user))
                return Unauthorized("Your account is locked. Please try again later.");

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,  
                lockoutOnFailure: true  // system will let you try few times then it will lock 
            );

            if (!result.Succeeded)
                return Unauthorized("Invalid login attempt");

            return Ok("User logged in successfully");
        }



        //logout 

        [HttpPost("logout")]
        [Authorize]  // you should be login to do  logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();  // delete the cookies and stop the session 

            return Ok("Logged out successfully");
        }


        //for  the role 

        [HttpGet("GetRoleByEmail")]
        public async Task<IActionResult> GetRoleByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any())
                return NotFound("Role not assigned");

            return Ok(roles.First()); //  "Admin" or "User"
        }


    }
}




