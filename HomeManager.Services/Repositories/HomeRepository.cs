using HomeManager.Data.Data.Context;
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
            return await _context.Homes
                .Include(h => h.Images)
                .ToListAsync();
                
        }

        public async Task<Home?> GetByIdAsync(Guid Id)
        {
            //return await _context.Homes.FindAsync(Id);
            return await _context.Homes.Include(x => x.Images).
                Include(x => x.Ratings)
                .FirstOrDefaultAsync(x => x.Id == Id);
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

        public async Task<ICollection<Home>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _context.Homes.Where(h => h.LandlordId == ownerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Home>> SearchAsync(string qry)
        {
            return await _context.Homes
                .Where(h => h.HomeName.Contains(qry) || h.HomeDescription.Contains(qry))
                .ToListAsync();
        }

        public async Task AddHomeImageAsync(HomeImage image)
        {
            _context.HomeImages.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task AddRatingAsync(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Rating>> GetRatingsForHomeAsync(Guid homeId)
        {
            return await _context.Ratings
                .Where(r => r.HomeId == homeId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
