using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeManager.Data.Data.Models;
using HomeManager.Services.Services;
using HomeManager.Services.Services.Interfaces;

namespace HomeManager.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) => _userService = userService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile(Guid? id)
        {
            User user;
            if (id == null)
            {
                user = await _userService.GetCurrentUserAsync();
            }
            else
            {
                user = await _userService.GetByIdAsync(id.Value);
            }
            if (user == null) return NotFound();

            var currentUserId = _userService.GetCurrentUserId();
            ViewBag.CanEdit = currentUserId.HasValue && currentUserId.Value == user.Id;
            return View("~/Views/Auth/Users/Profile.cshtml", user); 
        }

        [HttpPost]
        public async Task<IActionResult> Profile(User model)
        {
            var user = await _userService.GetCurrentUserAsync();
            user.FullName = model.FullName;
            user.Bio = model.Bio;
            user.AvatarUrl = model.AvatarUrl;
            await _userService.UpdateUserAsync(user);
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile(Guid? id)
        {
            User user;
            if (id == null)
            {
                user = await _userService.GetCurrentUserAsync();
            }
            else
            {
                user = await _userService.GetByIdAsync(id.Value);
            }
            if (user == null) return NotFound();

            var currentUserId = _userService.GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != user.Id)
                return Forbid(); 

            return View("~/Views/Auth/Users/EditProfile.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(User model)
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null || user.Id != model.Id)
                return Forbid();

            user.FullName = model.FullName;
            user.Bio = model.Bio;

            var file = Request.Form.Files["AvatarImage"];
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/AvatarImages");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                user.AvatarUrl = $"/AvatarImages/{uniqueFileName}";
            }

            await _userService.UpdateUserAsync(user);
            return RedirectToAction("Profile", new { id = user.Id });
        }
    }
}
