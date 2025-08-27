using AutoMapper;
using FluentAssertions;
using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Enums;
using HomeManager.Data.Data.ViewModels;
using HomeManager.Services.Repositories.Interfaces;
using HomeManager.Services.Services;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace HomeManager.UnitTests

{
    [TestFixture]
    public class HomeServiceTests
    {
        private Mock<IHomeRepository> _repoMock = null!;
        private Mock<IUserRepository> _userRepoMock = null!;
        private Mock<IConversationService> _convServiceMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IWebHostEnvironment> _envMock = null!;
        private HomeService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IHomeRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _convServiceMock = new Mock<IConversationService>();
            _mapperMock = new Mock<IMapper>();
            _envMock = new Mock<IWebHostEnvironment>();
            _service = new HomeService(_repoMock.Object, _userRepoMock.Object, _convServiceMock.Object, _mapperMock.Object, _envMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ReturnsHomeDtos()
        {
            var homes = new List<Home>
            {
                new Home
                {
                    Id = Guid.NewGuid(),
                    HomeName = "Test Home",
                    HomeLocation = "City",
                    HomeType = HomeType.Apartment,
                    HomeDescription = "Great",
                    HomeDealType = DealType.Rent,
                    HomePrice = 999,
                    LandlordId = Guid.NewGuid(),
                    AddedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow,
                }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(homes);

            var result = await _service.GetAllAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().HomeName.Should().Be("Test Home");
        }

        [Test]
        public async Task GetByIdAsync_ReturnsHomeDto_WhenFound()
        {
            var id = Guid.NewGuid();
            var home = new Home
            {
                Id = id,
                HomeName = "Villa",
                HomeLocation = "Beach",
                HomeType = HomeType.Apartment,
                HomeDescription = "Luxury",
                HomeDealType = DealType.Sale,
                HomePrice = 300000,
                LandlordId = Guid.NewGuid(),
                AddedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(home);

            var result = await _service.GetByIdAsync(id);

            result.Should().NotBeNull();
            result.HomeName.Should().Be("Villa");
        }

        [Test]
        public void GetByIdAsync_ThrowsException_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Home?)null);

            Func<Task> act = async () => await _service.GetByIdAsync(id);

            act.Should().ThrowAsync<Exception>().WithMessage("Home not found!");
        }

        [Test]
        public async Task CreateAsync_ReturnsNewHomeId()
        {
            var dto = new CreateHomeDto
            {
                HomeName = "New Home",
                HomeLocation = "Suburb",
                HomeType = HomeType.Apartment,
                HomeDescription = "Nice flat",
                HomeDealType = DealType.Rent,
                HomePrice = 1500,
                LandlordId = Guid.NewGuid()
            };

            Home? capturedHome = null;

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Home>()))
                .Callback<Home>(h => capturedHome = h)
                .Returns(Task.CompletedTask);

            var newId = await _service.CreateAsync(dto);

            capturedHome.Should().NotBeNull();
            capturedHome!.HomeName.Should().Be("New Home");
            newId.Should().Be(capturedHome.Id);
        }

        [Test]
        public async Task UpdateAsync_UpdatesHome_WhenFound()
        {
            var id = Guid.NewGuid();
            var home = new Home
            {
                Id = id,
                HomeName = "Old Name",
                HomeLocation = "Old Location",
                HomeType = HomeType.Apartment,
                HomeDescription = "Old Desc",
                HomeDealType = DealType.Rent,
                HomePrice = 1000,
                Region = "OldRegion",
                City = "OldCity",
                LandlordId = Guid.NewGuid(),
                AddedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            var editModel = new EditHomeViewModel
            {
                Id = id,
                HomeName = "Updated Name",
                HomeLocation = "Updated Location",
                HomeType = HomeType.House,
                HomeDescription = "Updated Desc",
                HomeDealType = DealType.Sale,
                HomePrice = 2000,
                Region = "NewRegion",
                City = "NewCity",
                Latitude = 1.23,
                Longitude = 4.56
            };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(home);
            _repoMock.Setup(r => r.UpdateAsync(home)).Returns(Task.CompletedTask);

            await _service.UpdateAsync(id, editModel);

            home.HomeName.Should().Be("Updated Name");
            home.HomeLocation.Should().Be("Updated Location");
            home.HomeType.Should().Be(HomeType.House);
            home.HomeDescription.Should().Be("Updated Desc");
            home.HomeDealType.Should().Be(DealType.Sale);
            home.HomePrice.Should().Be(2000);
            home.Region.Should().Be("NewRegion");
            home.City.Should().Be("NewCity");
            home.Latitude.Should().Be(1.23);
            home.Longitude.Should().Be(4.56);
        }

        [Test]
        public void UpdateAsync_ThrowsException_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Home?)null);

            Func<Task> act = async () => await _service.UpdateAsync(id, new EditHomeViewModel());

            act.Should().ThrowAsync<Exception>().WithMessage("Home not Found!");
        }

        [Test]
        public async Task DeleteAsync_DeletesHome_WhenFound()
        {
            var id = Guid.NewGuid();
            var home = new Home { Id = id };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(home);
            _repoMock.Setup(r => r.DeleteAsync(home)).Returns(Task.CompletedTask);

            await _service.DeleteAsync(id);

            _repoMock.Verify(r => r.DeleteAsync(home), Times.Once);
        }

        [Test]
        public void DeleteAsync_ThrowsException_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Home?)null);

            Func<Task> act = async () => await _service.DeleteAsync(id);

            act.Should().ThrowAsync<Exception>().WithMessage("Home not Found!");
        }

        [Test]
        public async Task AddRatingAsync_AddsRating_WhenHomeAndUserExist()
        {
            var homeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var home = new Home { Id = homeId, LandlordId = userId };
            var user = new User { Id = userId };

            _repoMock.Setup(r => r.GetByIdAsync(homeId)).ReturnsAsync(home);
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _repoMock.Setup(r => r.AddRatingAsync(It.IsAny<Rating>())).Returns(Task.CompletedTask);

            await _service.AddRatingAsync(homeId, userId, 5, "Great!");

            _repoMock.Verify(r => r.AddRatingAsync(It.Is<Rating>(rat => rat.Value == 5 && rat.Comment == "Great!")), Times.Once);
        }

        [Test]
        public void AddRatingAsync_ThrowsException_WhenHomeNotFound()
        {
            var homeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(homeId)).ReturnsAsync((Home?)null);

            Func<Task> act = async () => await _service.AddRatingAsync(homeId, userId, 5, "Great!");

            act.Should().ThrowAsync<Exception>().WithMessage("Home not found!");
        }

        [Test]
        public void AddRatingAsync_ThrowsException_WhenUserNotFound()
        {
            var homeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var home = new Home { Id = homeId, LandlordId = userId };
            _repoMock.Setup(r => r.GetByIdAsync(homeId)).ReturnsAsync(home);
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            Func<Task> act = async () => await _service.AddRatingAsync(homeId, userId, 5, "Great!");

            act.Should().ThrowAsync<Exception>().WithMessage("User not found!");
        }

        [Test]
        public void AddRatingAsync_ThrowsException_WhenRatingValueOutOfRange()
        {
            var homeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var home = new Home { Id = homeId, LandlordId = userId };
            var user = new User { Id = userId };
            _repoMock.Setup(r => r.GetByIdAsync(homeId)).ReturnsAsync(home);
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            Func<Task> act = async () => await _service.AddRatingAsync(homeId, userId, 0, "Bad!");

            act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Test]
        public async Task GetAverageRatingAsync_ReturnsAverage()
        {
            var homeId = Guid.NewGuid();
            var ratings = new List<Rating>
            {
                new Rating { Value = 4 },
                new Rating { Value = 2 }
            };
            _repoMock.Setup(r => r.GetRatingsForHomeAsync(homeId)).ReturnsAsync(ratings);

            var avg = await _service.GetAverageRatingAsync(homeId);

            avg.Should().Be(3.0);
        }

        [Test]
        public async Task GetAverageRatingAsync_ReturnsZero_WhenNoRatings()
        {
            var homeId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetRatingsForHomeAsync(homeId)).ReturnsAsync(new List<Rating>());

            var avg = await _service.GetAverageRatingAsync(homeId);

            avg.Should().Be(0.0);
        }
    }
}

