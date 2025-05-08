using HomeManager.Data.Data.Dtos;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeManager.Controllers
{
    public class HomesController : Controller
    {
        private readonly IHomeService _homeService;

        public HomesController(IHomeService homeService)
        {
            _homeService = homeService;
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
    }
}
