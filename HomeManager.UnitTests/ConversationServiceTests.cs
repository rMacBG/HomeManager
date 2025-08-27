using FluentAssertions;
using HomeManager.Data.Data.Models;
using HomeManager.Services.Repositories.Interfaces;
using HomeManager.Services.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.UnitTests
{
    [TestFixture]
    public class ConversationServiceTests
    {
        private Mock<IConversationRepository> _convRepoMock = null!;
        private Mock<IUserRepository> _userRepoMock = null!;
        private Mock<IHomeRepository> _homeRepoMock = null!;
        private ConversationService _service = null!;

        [SetUp]
        public void Setup()
        {
            _convRepoMock = new Mock<IConversationRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _homeRepoMock = new Mock<IHomeRepository>();
            _service = new ConversationService(_userRepoMock.Object, _convRepoMock.Object, _homeRepoMock.Object);
        }

        [Test]
        public async Task GetOrCreateConversationAsync_ReturnsConversationId()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var conversations = new List<Conversation>();
            _convRepoMock.Setup(r => r.GetByUserIdAsync(userId1)).ReturnsAsync(conversations);

            var result = await _service.GetOrCreateConversationAsync(userId1, userId2);

            result.Should().NotBe(Guid.Empty);
        }

        [Test]
        public async Task GetConversationDetailsAsync_ReturnsConversation_WhenFound()
        {
            var convId = Guid.NewGuid();
            var conversation = new Conversation { Id = convId };
            _convRepoMock.Setup(r => r.GetByIdAsync(convId)).ReturnsAsync(conversation);

            var result = await _service.GetConversationDetailsAsync(convId);

            result.Should().NotBeNull();
            result.Id.Should().Be(convId);
        }
    }
}
