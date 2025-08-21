using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models.Enums;
using HomeManager.Data.Data.ViewModels;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HomeManager.Controllers
{

    public class SearchController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IUserService _userService;

        public SearchController(IHomeService homeService, IUserService userService)
        {
            _homeService = homeService;
            _userService = userService;
        }

        [HttpGet]
        [Route("Search/GetResults")]
        public async Task<IActionResult> GetResults(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(new SearchResultDto[0]);
            }

            var homes = await _homeService.SearchHomesAsync(query);
            var users = await _userService.SearchUsersAsync(query);

            var homeResults = homes.Select(h => new SearchResultDto { Id = h.Id, Name = h.HomeName, Type = "Home" });
            foreach (var h in homeResults)
            {
                System.Diagnostics.Debug.WriteLine($"SearchResultDto: Id={h.Id}, Name={h.Name}, Type={h.Type}");
            }
            var userResults = users.Select(u => new SearchResultDto { Id = u.Id, Name = u.Username, Type = "User" });
            var combined = homeResults.Concat(userResults);

            return Json(combined.ToList());
        }

        public async Task<IActionResult> List(string query, string homeType,decimal? minPrice, decimal? maxPrice, string? region, string? city)
        {
            var homes = await _homeService.FilteredSearchAsync(query, homeType, minPrice, maxPrice, region, city);
            var viewModel = new SearchResultsViewModel
            {
                Query = query,
                HomeType = homeType,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                HomeResults = homes.ToList(),
                UserResults = string.IsNullOrWhiteSpace(query) ? new List<UserDto>() : (await _userService.SearchUsersAsync(query)).ToList()
            };
            return View(viewModel);
        }
        [Route("Search/AdvancedSearchList")]
        public async Task<IActionResult> AdvancedSearchList(string query, string homeType, int? minPrice, int? maxPrice, string? region, string? city)
        {
            var results = await _homeService.FilteredSearchAsync(query, homeType, minPrice, maxPrice, region, city);
            return View(results);
        }
    }

}

