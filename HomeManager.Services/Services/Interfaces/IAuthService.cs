using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Dtos.Results;
using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string username, string password);
        Task<AuthResult> RegisterAsync(RegisterUserDto dto);
        Task<User> FindUserByEmailAsync(string email);
        Task SavePasswordResetTokenAsync(Guid userId, string token);
        Task SendPasswordResetEmailAsync(string email, string resetLink);
        Task<Guid?> ValidatePasswordResetTokenAsync(string token);
        Task UpdatePasswordAsync(Guid userId, string newPassword);
    }
}
