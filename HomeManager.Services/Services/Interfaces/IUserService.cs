using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetCurrentUserAsync();
        Guid? GetCurrentIdAsync();
        bool IsAuthenticated();
        string? GetCurrentUsername();
    }
}
