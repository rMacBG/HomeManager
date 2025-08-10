using NUnit.Framework;
using Moq;
using FluentAssertions;
using HomeManager.Services.Services;
using HomeManager.Services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models.Enums;
using HomeManager.Data.Data.Models;
namespace HomeManager.UnitTests

{
        public class HomeServiceTests
        {
            private Mock<IHomeRepository> _repoMock = null!;
            private HomeService _service = null!;

            [SetUp]
            public void Setup()
            {
                _repoMock = new Mock<IHomeRepository>();
               // _service = new HomeService(_repoMock.Object);
            }

            [Test]
            public async Task GetAllAsync_ShouldReturnListOfDtos()
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

                var resultList = result.ToList();
            resultList[0].HomeName.Should().Be("Test Home");
            }

            [Test]
            public async Task GetByIdAsync_ShouldReturnDto_WhenFound()
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

                result.HomeName.Should().Be("Villa");
            }

            [Test]
            public async Task CreateAsync_ShouldReturnId()
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
            public void UpdateAsync_ShouldThrow_WhenNotFound()
            {
                var id = Guid.NewGuid();
                _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Home?)null);

               // Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(id, new CreateHomeDto()));
            }

            [Test]
            public void DeleteAsync_ShouldThrow_WhenNotFound()
            {
                var id = Guid.NewGuid();
                _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Home?)null);

                Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(id));
            }
        }
    }

