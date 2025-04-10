using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Services.Repositories.Interfaces;
using HomeManager.Services.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) 
            {
                throw new UnauthorizedAccessException("Invalid Credentials");
            }
            return GenerateJwtToken(user);
        }

        public async Task RegisterAsync(RegisterUserDto dto)
        {
            var exists = await _userRepository.ExistsByUsernameAsync(dto.Username);
            if(exists)
            {
                throw new InvalidOperationException("Username already exists!");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
            };

            await _userRepository.AddAsync(user);
        }

        private string GenerateJwtToken(User user)
        {
            
        }
    }
}
