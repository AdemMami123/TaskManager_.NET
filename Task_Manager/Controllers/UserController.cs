using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task_Manager.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Task_Manager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserWithRoleViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = string.Join(", ", roles)
                });
            }

            return View(userList);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Role))
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var result = await _userManager.AddToRoleAsync(user, model.Role);

            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update role" });
            }
        }

        // ✅ DELETE USER FUNCTIONALITY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserId))
            {
                return Json(new { success = false, message = "Invalid user ID." });
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Ensure only Admins can delete users
            if (!User.IsInRole("Admin"))
            {
                return Json(new { success = false, message = "Unauthorized action." });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Failed to delete user." });
            }
        }
    }
}
