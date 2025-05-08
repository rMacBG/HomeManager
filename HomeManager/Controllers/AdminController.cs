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
        public async Task<IActionResult> Uesrs()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        //[HttpPost]
        //public async Task<IActionResult> ChangeRole(Guid userId, Role newRole)
        //{
        //    await _userService.ChangeUserRoleAsync(userId, newRole);
        //    return RedirectToAction("Users");
        //}
    }
}
