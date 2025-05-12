using HomeManager.Data.Data.Dtos;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeManager.Controllers
{
    public class HomesController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IUserService _userService;

        public HomesController(IHomeService homeService, IUserService userService)
        {
            _homeService = homeService;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var homes = await _homeService.GetAllAsync();
            if (homes == null)
                return View(new List<HomeDto>());
            return View(homes);
        }


        public async Task<IActionResult> Details(Guid id)
        {
            var home = _homeService.GetByIdAsync(id);
            if (home == null) 
                return NotFound();
            return View(home);
        }

        [Authorize(Roles = "Landlord,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Landlord,Admin")]
        public async Task<IActionResult> Create(CreateHomeDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _homeService.CreateAsync(dto);
            return RedirectToAction("Index");
        }
    }
}
