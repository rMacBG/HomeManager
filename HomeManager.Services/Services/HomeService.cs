using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Interfaces;
using HomeManager.Services.Repositories.Interfaces;
using HomeManager.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _repository;
        private readonly IUserRepository _userRepository;

        public HomeService(IHomeRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }
        public async Task<ICollection<HomeDto>> GetAllAsync()
        {
            var homes = await _repository.GetAllAsync();
            return homes.Select(x => new HomeDto
            {
                Id = x.Id,
                HomeName = x.HomeName,
                HomeLocation = x.HomeLocation,
                HomeType = x.HomeType,
                HomeDescription = x.HomeDescription,
                HomeDealType = x.HomeDealType,
                HomePrice = x.HomePrice,
                LandlordId = x.LandlordId,
            }).ToList();
        }

        public async Task<HomeDto> GetByIdAsync(Guid id)
        {
            var home = await _repository.GetByIdAsync(id)
                ?? throw new Exception("Home not found!");

            var homeById = new HomeDto
            {
                Id = home.Id,
                HomeName = home.HomeName,
                HomeLocation = home.HomeLocation,
                HomeType = home.HomeType,
                HomeDescription = home.HomeDescription,
                HomeDealType= home.HomeDealType,
                HomePrice = home.HomePrice,
                LandlordId= home.LandlordId,
            };

            return homeById;
        }

        public async Task<IEnumerable<HomeDto>> GetByOwnerIdsync(Guid ownerId)
        {
            var homes = await _repository.GetByOwnerIdAsync(ownerId);

            return homes.Select(home => new HomeDto
            {
                Id = home.Id, //this line trolled me this whole time reeeeeeeeeeee
                HomeName = home.HomeName,
                HomeLocation = home.HomeLocation,
                HomeType = home.HomeType,
                HomeDescription = home.HomeDescription,
                HomeDealType = home.HomeDealType,
                HomePrice = home.HomePrice,
                LandlordId = home.LandlordId,
            }) ?? new List<HomeDto>();
        }

        public async Task<HomeDto> EditAsync(HomeDto dto)
        {

            var home = new HomeDto
            {
                Id = dto.Id,
                HomeName = dto.HomeName,
                HomeLocation = dto.HomeLocation,
                HomeType = dto.HomeType,
                HomeDescription = dto.HomeDescription,
                HomeDealType = dto.HomeDealType,
                HomePrice = dto.HomePrice,
                LandlordId = dto.LandlordId
            };
            
            return home;
        }
        public async Task<Guid> CreateAsync(CreateHomeDto dto)
        {
            var home = new Home
            {
                Id = Guid.NewGuid(),
                HomeName = dto.HomeName,
                HomeLocation = dto.HomeLocation,
                HomeType = dto.HomeType,
                HomeDescription = dto.HomeDescription,
                HomeDealType=dto.HomeDealType,
                HomePrice = dto.HomePrice,
                LandlordId = dto.LandlordId,
                AddedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,

            };
            await _repository.AddAsync(home);
            return home.Id;
        }

        public async Task UpdateAsync(Guid id, CreateHomeDto dto)
        {
            var home = await _repository.GetByIdAsync(id)
                ?? throw new Exception("Home not Found!");
            
           // var landlord = await _userRepository.GetByIdAsync(home.LandlordId);
            
            home.HomeName = dto.HomeName;
            home.HomeLocation = dto.HomeLocation;
            home.HomeType = dto.HomeType;
            home.HomeDescription = dto.HomeDescription;
            home.HomeDealType = dto.HomeDealType;
            home.HomePrice = dto.HomePrice;
            home.LandlordId = dto.LandlordId;
            home.LastModifiedAt = DateTime.UtcNow;

            
            await _repository.UpdateAsync(home);
        }

        public async Task DeleteAsync(Guid id)
        {
            var home = await _repository.GetByIdAsync(id)
                ?? throw new Exception("Home not Found!");

            await _repository.DeleteAsync(home);
        }

        
    }
}
