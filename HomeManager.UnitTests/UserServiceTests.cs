using FluentAssertions;
using HomeManager.Data.Data.Models;
using HomeManager.Services.Repositories.Interfaces;
using HomeManager.Services.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.UnitTests
{
    
        [TestFixture]
        public class UserServiceTests
        {
            private Mock<IHttpContextAccessor> _httpContextMock = null!;
            private Mock<IUserRepository> _userRepoMock = null!;
            private UserService _service = null!;

            [SetUp]
            public void Setup()
            {
                _httpContextMock = new Mock<IHttpContextAccessor>();
                _userRepoMock = new Mock<IUserRepository>();
                _service = new UserService(_httpContextMock.Object, _userRepoMock.Object);
            }

            [Test]
            public async Task GetByIdAsync_ReturnsUser_WhenFound()
            {
                var userId = Guid.NewGuid();
                var user = new User { Id = userId, Username = "testuser" };
                _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

                var result = await _service.GetByIdAsync(userId);

                result.Should().NotBeNull();
                result.Username.Should().Be("testuser");
            }

            [Test]
            public async Task GetCurrentUserAsync_ReturnsUser_WhenAuthenticated()
            {
                var userId = Guid.NewGuid();
                var user = new User { Id = userId, Username = "currentuser" };
                _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
                var identity = new ClaimsIdentity(claims, "TestAuth");
                var principal = new ClaimsPrincipal(identity);
                var context = new DefaultHttpContext { User = principal };
                _httpContextMock.Setup(x => x.HttpContext).Returns(context);

                var result = await _service.GetCurrentUserAsync();

                result.Should().NotBeNull();
                result.Username.Should().Be("currentuser");
            }

            [Test]
            public void IsAuthenticated_ReturnsTrue_WhenUserIsAuthenticated()
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
                var identity = new ClaimsIdentity(claims, "TestAuth");
                var principal = new ClaimsPrincipal(identity);
                var context = new DefaultHttpContext { User = principal };
                _httpContextMock.Setup(x => x.HttpContext).Returns(context);

                var result = _service.IsAuthenticated();

                result.Should().BeTrue();
            }
        }
   }

