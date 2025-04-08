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
        Task<ICollection<Home>> GetAllAsync();

        Task<Home> GetByIdAsync(Guid Id);

        Task AddAsync(Home home);
        Task UpdateAsync(Home home);

        Task DeleteAsync(Home Home);

    }
}
