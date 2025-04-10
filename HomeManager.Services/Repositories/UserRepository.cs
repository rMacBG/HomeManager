﻿using HomeManager.Data.Data.Context;
using HomeManager.Data.Data.Models;
using HomeManager.Services.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HomeManagerDbContext _context;

        public UserRepository(HomeManagerDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUsernameAsync(string username)
        {
            return await _context
                .Users
                .FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .FindAsync(id);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context
                .Users
                .AnyAsync(x => x.Username == username);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
