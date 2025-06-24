using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities.UserMangment;

namespace APITast.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] //only admin 
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // to add new admin 
        // POST: api/Admin/MakeAdmin/{userId}
        [HttpPost("MakeAdmin/{userId}")]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
                return BadRequest("User is already an Admin.");

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User promoted to Admin successfully.");
        }

        //show all users 
        // GET: api/Admin/users
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }

        // delete users 
        // DELETE: api/Admin/DeleteUser/{userId}
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User deleted successfully.");
        }

        // search by email 
        // GET: api/Admin/FindByEmail?email=example@mail.com
        [HttpGet("FindByEmail")]
        public async Task<IActionResult> FindByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        // remove admin role 
        // POST: api/Admin/RemoveAdmin/{userId}
        [HttpPost("RemoveAdmin/{userId}")]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Admin role removed successfully.");
        }
    }
}