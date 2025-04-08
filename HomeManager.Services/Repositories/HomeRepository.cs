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
    public class HomeRepository : IHomeRepository
    {
        private readonly HomeManagerDbContext _context;

        public HomeRepository(HomeManagerDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Home>> GetAllAsync()
        { 
           return await _context.Homes.ToListAsync();
        }

        public async Task<Home?> GetByIdAsync(Guid Id)
        {
            return await _context.Homes.FindAsync(Id);
        }


        public async Task AddAsync(Home home)
        {
            await _context.Homes.AddAsync(home);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateAsync(Home home)
        {
            _context.Homes.Update(home);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Home Home)
        {
            _context.Homes.Remove(Home);
            await _context.SaveChangesAsync();
        }
 
    }
}
