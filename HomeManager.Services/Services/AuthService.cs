using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Dtos.Results;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Enums;
using HomeManager.Services.Repositories.Interfaces;
using HomeManager.Services.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

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

        public async Task<User?> FindUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string> { "Username or password are incorrect!" }
                };
            }
            return new AuthResult {
                Success = true,
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role.ToString(),
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<AuthResult> RegisterAsync(RegisterUserDto dto)
        {
            var exists = await _userRepository.ExistsByUsernameAsync(dto.Username);
            if(exists)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string> { "Username already exists!" }
                };
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role = Role.User
            };

            await _userRepository.AddAsync(user);

            return new AuthResult
            {
                Success = true,
                Token = GenerateJwtToken(user)
            };
        }

        public async Task SavePasswordResetTokenAsync(Guid userId, string token)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return;
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour
            await _userRepository.UpdateAsync(user);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]);
            var smtpUser = _configuration["Smtp:User"];
            var smtpPass = _configuration["Smtp:Pass"];
            var fromEmail = _configuration["Smtp:From"];

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(fromEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Password Reset Request";
            message.Body = new TextPart("plain")
            {
                Text = $"Click the link to reset your password: {resetLink}"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.SslOnConnect); // For port 465
            await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task UpdatePasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<Guid?> ValidatePasswordResetTokenAsync(string token)
        {
            var user = await _userRepository.GetAllAsync();
            var match = user.FirstOrDefault(u => u.PasswordResetToken == token && u.PasswordResetTokenExpiry > DateTime.UtcNow);
            return match?.Id;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
