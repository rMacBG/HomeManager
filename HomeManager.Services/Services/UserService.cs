using HomeManager.Data.Data.Models;
using HomeManager.Services.Repositories.Interfaces;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services
{
    public class UserService : IUserService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public UserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }
        public Guid? GetCurrentIdAsync()
        {
            var userIdstr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdstr, out var userId))
            {
                return userId;
            }
            return null;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var userId = GetCurrentIdAsync();
            return userId == null ? null : await _userRepository.GetByIdAsync(userId.Value);
        }   

        public string? GetCurrentUsername()
        {
           return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        }
    }
}
