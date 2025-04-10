using HomeManager.Data.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string username, string password);
        Task RegisterAsync(RegisterUserDto dto);
    }
}
