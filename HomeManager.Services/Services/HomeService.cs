using AutoMapper;
using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Interfaces;
using HomeManager.Data.Data.ViewModels;
using HomeManager.Services.Repositories.Interfaces;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConversationService _conversationService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public HomeService(IHomeRepository repository, IUserRepository userRepository, IConversationService conversationService, IMapper mapper, IWebHostEnvironment env)
        {
            _homeRepository = repository;
            _userRepository = userRepository;
            _conversationService = conversationService;
            _mapper = mapper;

            _env = env;
        }

        public async Task<ICollection<HomeDto>> GetAllAsync()
        {
            var homes = await _homeRepository.GetAllAsync();
            return homes.Select(x => new HomeDto
            {
                Id = x.Id,
                HomeName = x.HomeName,
                HomeLocation = x.HomeLocation,
                HomeType = x.HomeType,
                HomeDescription = x.HomeDescription,
                HomeDealType = x.HomeDealType,
                HomePrice = x.HomePrice,
                Region = x.Region,
                City = x.City,
                LandlordId = x.LandlordId,
                Images = x.Images?.Select(img => new HomeImageDto
                { FilePath = img.FilePath }).ToList() ?? new List<HomeImageDto>(),
                Ratings = x.Ratings.ToList(),
                Latitude = x.Latitude,
                Longitude = x.Longitude,
            }).ToList();
        }

        public async Task<HomeDto> GetByIdAsync(Guid id)
        {
            var home = await _homeRepository.GetByIdAsync(id)
                ?? throw new Exception("Home not found!");

            var homeById = new HomeDto
            {
                Id = home.Id,
                HomeName = home.HomeName,
                HomeLocation = home.HomeLocation,
                HomeType = home.HomeType,
                HomeDescription = home.HomeDescription,
                HomeDealType = home.HomeDealType,
                Region = home.Region,
                City = home.City,
                HomePrice = home.HomePrice,
                LandlordId = home.LandlordId,
                Ratings = home.Ratings.ToList(),
                Images = home.Images.Select(img => new HomeImageDto
                {
                    FilePath = img.FilePath,
                }).ToList(),
                Latitude = home.Latitude,
                Longitude = home.Longitude

            };

            return homeById;
        }

        public async Task<IEnumerable<HomeDto>> GetByOwnerIdsync(Guid ownerId)
        {
            var homes = await _homeRepository.GetByOwnerIdAsync(ownerId);

            return homes.Select(home => new HomeDto
            {
                Id = home.Id, //this line trolled me this whole time reeeeeeeeeeee
                HomeName = home.HomeName,
                HomeLocation = home.HomeLocation,
                HomeType = home.HomeType,
                HomeDescription = home.HomeDescription,
                HomeDealType = home.HomeDealType,
                HomePrice = home.HomePrice,
                Region = home.Region,
                City = home.City,
                LandlordId = home.LandlordId,
                Images = home.Images?.Select(img => new HomeImageDto { FilePath = img.FilePath }).ToList() ?? new List<HomeImageDto>(),
                Ratings = home.Ratings.ToList(),
                Latitude = home.Latitude,
                Longitude = home.Longitude,
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
                Region = dto.Region,
                City = dto.City,
                LandlordId = dto.LandlordId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };

            return home;
        }
        public async Task<Guid> CreateAsync(CreateHomeDto dto)
        {
            var home = new Home
            {
                Id = Guid.NewGuid(),
                HomeName = dto.HomeName,
                HomeLocation = string.IsNullOrWhiteSpace(dto.HomeLocation) ? "Default Location" : dto.HomeLocation,
                HomeType = dto.HomeType,
                HomeDescription = dto.HomeDescription,
                HomeDealType = dto.HomeDealType,
                HomePrice = dto.HomePrice,
                Region = dto.Region,
                City = dto.City,
                LandlordId = dto.LandlordId,
                AddedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,

            };

            if (dto.UploadedImages != null)
            {
                foreach (var image in dto.UploadedImages)
                {
                    if (image.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        home.Images.Add(new HomeImage
                        {
                            FilePath = "/uploads/" + uniqueFileName
                        });
                    }
                }
            }
            await _homeRepository.AddAsync(home);
            return home.Id;
        }

        public async Task UpdateAsync(Guid id, EditHomeViewModel dto)
        {
            var home = await _homeRepository.GetByIdAsync(id)
                ?? throw new Exception("Home not Found!");

            home.HomeName = dto.HomeName;
            home.HomeLocation = string.IsNullOrWhiteSpace(dto.HomeLocation) ? "Default Location" : dto.HomeLocation;
            home.HomeType = dto.HomeType;
            home.HomeDescription = dto.HomeDescription;
            home.HomeDealType = dto.HomeDealType;
            home.HomePrice = dto.HomePrice;
            home.Region = dto.Region;
            home.City = dto.City;
            home.LastModifiedAt = DateTime.UtcNow;
            home.Latitude = dto.Latitude;
            home.Longitude = dto.Longitude;

            if (dto.ImagesToRemove != null && dto.ImagesToRemove.Any())
            {
                home.Images.RemoveAll(img => dto.ImagesToRemove.Contains(img.FilePath));
                foreach (var filePath in dto.ImagesToRemove)
                {
                    var physicalPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
                    if (System.IO.File.Exists(physicalPath))
                        System.IO.File.Delete(physicalPath);
                }
            }


            if (dto.UploadedImages != null && dto.UploadedImages.Any())
            {
                foreach (var image in dto.UploadedImages)
                {
                    if (image.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        home.Images.Add(new HomeImage
                        {
                            FilePath = "/uploads/" + uniqueFileName
                        });
                    }
                }
            }

            await _homeRepository.UpdateAsync(home);
        }

        public async Task DeleteAsync(Guid id)
        {
            var home = await _homeRepository.GetByIdAsync(id)
                ?? throw new Exception("Home not Found!");

            await _homeRepository.DeleteAsync(home);
        }

        public async Task<IEnumerable<HomeDto>> SearchHomesAsync(string query)
        {
            var homes = await _homeRepository.SearchAsync(query);
            return homes.Select(h => new HomeDto
            {
                Id = h.Id,
                HomeName = h.HomeName,

            });
        }

        public async Task<string> UploadHomeImageAsync(Guid homeId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid image file.");

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var homeImage = new HomeImage
            {
                HomeId = homeId,
                FilePath = "/uploads/" + fileName
            };

            await _homeRepository.AddHomeImageAsync(homeImage);

            return homeImage.FilePath;
        }

        public async Task<List<HomeDto>> GetLatestEstatesAsync(int count)
        {
            var homes = await _homeRepository.GetAllAsync();
            return homes
                .OrderByDescending(x => x.AddedAt)
                .Take(count)
                .Select(x => new HomeDto
                {
                    Id = x.Id,
                    HomeName = x.HomeName,
                    HomeLocation = x.HomeLocation,
                    HomeType = x.HomeType,
                    HomeDescription = x.HomeDescription,
                    HomeDealType = x.HomeDealType,
                    HomePrice = x.HomePrice,
                    Region = x.Region,
                    City = x.City,
                    LandlordId = x.LandlordId,
                    Images = x.Images?.Select(img => new HomeImageDto { FilePath = img.FilePath }).ToList() ?? new List<HomeImageDto>(),
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                })
                .ToList();
        }

        public async Task<IEnumerable<HomeDto>> FilteredSearchAsync(string query, string homeType, decimal? minPrice, decimal? maxPrice, string? region, string? city)
        {
            var estates = await _homeRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(query))
                estates = estates.Where(e => e.HomeName.Contains(query, StringComparison.OrdinalIgnoreCase)
                                     || e.HomeDescription.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(homeType))
                estates = estates.Where(e => e.HomeType.ToString() == homeType).ToList();

            if (minPrice.HasValue)
                estates = estates.Where(e => e.HomePrice >= minPrice.Value).ToList();

            if (maxPrice.HasValue)
                estates = estates.Where(e => e.HomePrice <= maxPrice.Value).ToList();

            if (!string.IsNullOrEmpty(region))
                estates = estates.Where(e => e.Region == region).ToList();

            if (!string.IsNullOrEmpty(city))
                estates = estates.Where(e => e.City == city).ToList();

            return estates.Select(e => new HomeDto
            {
                Id = e.Id,
                HomeName = e.HomeName,
                HomeLocation = e.HomeLocation,
                HomeType = e.HomeType,
                HomeDescription = e.HomeDescription,
                HomeDealType = e.HomeDealType,
                HomePrice = e.HomePrice,
                Region = e.Region,
                City = e.City,
                LandlordId = e.LandlordId,
                Images = e.Images?.Select(img => new HomeImageDto { FilePath = img.FilePath }).ToList() ?? new List<HomeImageDto>(),
                Latitude = e.Latitude,
                Longitude = e.Longitude,
            });
        }

        public async Task AddRatingAsync(Guid homeId, Guid userId, int ratingValue, string? comment)
        {
            var home = await _homeRepository.GetByIdAsync(homeId)
                ?? throw new Exception("Home not found!");
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found!");
            if (ratingValue < 1 || ratingValue > 5)
                throw new ArgumentOutOfRangeException(nameof(ratingValue), "Rating value must be between 1 and 5.");
            var rating = new Rating
            {
                Id = Guid.NewGuid(),
                HomeId = homeId,
                UserId = userId,
                Value = ratingValue,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };
            await _homeRepository.AddRatingAsync(rating);
        }

        public async Task<double> GetAverageRatingAsync(Guid homeId)
        {
            var ratings = await _homeRepository.GetRatingsForHomeAsync(homeId);
            if (ratings == null || !ratings.Any())
                return 0.0;
            return ratings.Average(r => r.Value);
        }
    }
}
