﻿using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services.Interfaces
{
    public interface IHomeService
    {
        Task<ICollection<HomeDto>> GetAllAsync();

        Task<IEnumerable<HomeDto>> GetByOwnerIdsync(Guid ownerId);

        //Task<HomeDetailsViewModel> GetHomeDetailsAsync(Guid homeId, Guid userId);
        Task<HomeDto> GetByIdAsync(Guid id);
        Task<HomeDto> EditAsync(HomeDto dto);
        Task<Guid> CreateAsync(CreateHomeDto dto);
        Task UpdateAsync(Guid id, CreateHomeDto dto);
        Task DeleteAsync(Guid id);

        Task<IEnumerable<HomeDto>> SearchHomesAsync(string query);
    }
}
