using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Repositories.Interfaces
{
    public interface IHomeRepository
    {
        public Task<ICollection<Home>> GetAllAsync();

        public Task<Home> GetByIdAsync(Guid Id);
        public Task<ICollection<Home>> GetByOwnerIdAsync(Guid ownerId);

        public Task AddAsync(Home home);
        public Task UpdateAsync(Home home);

        public Task DeleteAsync(Home Home);

    }
}
