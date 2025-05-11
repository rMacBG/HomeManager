using HomeManager.Data.Data.Models.Enums;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(Guid userId, Role newRole)
        {
            await _userService.UpdateUserRoleAsync(userId, newRole);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteRole(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction("Index");

        }
    }
}
