﻿using HomeManager.Data.Data.Dtos;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }

}
    

