﻿using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetAllAsync();
        Task<User?> GetUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<bool> ExistsByUsernameAsync(string username);
        Task AddAsync(User user);
        Task<IEnumerable<User>> SearchUsersAsync(string query);
        Task UpdateAsync(User user);

        Task DeleteAsync(Guid id);
    }
}
