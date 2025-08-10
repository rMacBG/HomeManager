using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<ICollection<UserDto>> GetAllAsync();

        Task<User> GetByIdAsync(Guid id);
        Task<User?> GetCurrentUserAsync();
        Guid? GetCurrentIdAsync();
        bool IsAuthenticated();
        string? GetCurrentUsername();

        Task UpdateUserRoleAsync(Guid userId, Role newRole);

        Task DeleteUserAsync(Guid id);

        Task<IEnumerable<UserDto>> SearchUsersAsync(string query);

        Guid? GetCurrentUserId();
        
    }
}
