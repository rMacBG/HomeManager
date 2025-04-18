﻿using HomeManager.Data.Data.Dtos;
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
        Task<HomeDto> GetByIdAsync(Guid id);

        Task<Guid> CreateAsync(CreateHomeDto dto);
        Task UpdateAsync(Guid id, CreateHomeDto dto);
        Task DeleteAsync(Guid id);
    }
}
