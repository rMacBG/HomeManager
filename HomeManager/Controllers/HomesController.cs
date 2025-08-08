using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.ViewModels;
using HomeManager.Services.Services;
using HomeManager.Services.Services.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Composition.Convention;
using System.Security.Claims;

namespace HomeManager.Controllers
{
    //[Authorize(Roles = "Landlord,Seller, User")]
    public class HomesController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;

        public HomesController(IHomeService homeService, IHttpContextAccessor httpContextAccessor, IUserService userService, IConversationService conversationService, IMessageService messageService)
        {
            _homeService = homeService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _conversationService = conversationService;
            _messageService = messageService;

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
        [Route("Homes/Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var home = await _homeService.GetByIdAsync(id);

            if (home == null)
                return NotFound();

            var viewModel = new HomeDetailsViewModel
            {
                Home = home,
            };

            return View(viewModel);
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
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var home = await _homeService.GetByIdAsync(id);
            if (home == null)
                return NotFound();

            
            var model = new EditHomeViewModel
            {
                Id = home.Id,
                HomeName = home.HomeName,
                HomeDescription = home.HomeDescription,
                HomeLocation = home.HomeLocation,
                HomePrice = home.HomePrice,
                HomeType = home.HomeType,
                HomeDealType = home.HomeDealType,
                // Optionally, add existing images if you want to show them
                ExistingImages = home.Images?.Select(i => i.FilePath).ToList() ?? new List<string>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Landlord,Seller,Admin")]
        public async Task<IActionResult> Edit(Guid id, CreateHomeDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var userId = User.FindFirst("nameidentifier")?.Value;

            await _homeService.UpdateAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> ChatBoxPartial(Guid homeId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Register", "Auth");

            var viewModel = await _conversationService.GetChatBoxViewModelAsync(homeId, userId.Value);
            if (viewModel == null)
                return NotFound();

            return PartialView("_ChatBox", viewModel);
        }
    }
}
