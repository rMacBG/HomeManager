using HomeManager.Data.Data.Dtos;
using HomeManager.Services.Services;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Composition.Convention;
using System.Security.Claims;

namespace HomeManager.Controllers
{
    [Authorize(Roles = "Landlord,Seller")]
    public class HomesController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public HomesController(IHomeService homeService, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _homeService = homeService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            
        }

        public async Task<IActionResult> Index()
        {
             var homes = await _homeService.GetAllAsync();
            return View(homes);
        }

        public async Task<IActionResult> MyProperties()
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var homes = await _homeService.GetByOwnerIdsync(userId);

            return View(homes);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var home = await _homeService.GetByIdAsync(id);
            if (home == null)
            {
                return NotFound();
            }
            return View(home);
        }
        
        public IActionResult Create()
        {
            return View();  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Landlord,Seller,Admin")]
        public async Task<IActionResult> Create(CreateHomeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            dto.LandlordId = userId;
            await _homeService.CreateAsync(dto);
            return RedirectToAction(nameof(Index)); 
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var home = await _homeService.GetByIdAsync(id);
            var dto = new CreateHomeDto
            {
                HomeName = home.HomeName,
                HomeLocation = home.HomeLocation,
                HomeType = home.HomeType,
                HomeDescription = home.HomeDescription,
                HomeDealType = home.HomeDealType,
                HomePrice = home.HomePrice,
                LandlordId = home.LandlordId
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Landlord,Seller,Admin")]
        public async Task<IActionResult> Edit(Guid id, CreateHomeDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _homeService.UpdateAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var home = await _homeService.GetByIdAsync(id);
            return View(home);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _homeService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
