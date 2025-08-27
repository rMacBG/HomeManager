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

        [Test]
        public async Task StartConversationAsync_CreatesConversationWithParticipants()
        {
            var participantIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            Conversation? capturedConv = null;
            _convRepoMock.Setup(r => r.AddAsync(It.IsAny<Conversation>()))
                .Callback<Conversation>(c => capturedConv = c)
                .Returns(Task.CompletedTask);

            var result = await _service.StartConversationAsync(participantIds);

            capturedConv.Should().NotBeNull();
            capturedConv!.UsersConversations.Select(uc => uc.UserId).Should().BeEquivalentTo(participantIds);
            result.Should().Be(capturedConv.Id);
        }

        [Test]
        public async Task GetUserConversationsForUserIdAsync_ReturnsConversationDtos()
        {
            var userId = Guid.NewGuid();
            var conversations = new List<Conversation>
            {
                new Conversation
                {
                    Id = Guid.NewGuid(),
                    StartedAt = DateTime.UtcNow,
                    UsersConversations = new List<UserConversation>
                    {
                        new UserConversation { UserId = userId },
                        new UserConversation { UserId = Guid.NewGuid() }
                    }
                }
            };
            _convRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(conversations);

            var result = await _service.GetUserConversationsForUserIdAsync(userId);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().ParticipantsIds.Should().Contain(userId);
        }

        [Test]
        public async Task GetOrCreateConversationForHomeAsync_Throws_WhenUserIsLandlord()
        {
            var homeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var home = new Home { Id = homeId, LandlordId = userId };
            _homeRepoMock.Setup(r => r.GetByIdAsync(homeId)).ReturnsAsync(home);

            Func<Task> act = async () => await _service.GetOrCreateConversationForHomeAsync(homeId, userId);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot send messages to yourself.");
        }

        [Test]
        public async Task GetChatBoxViewModelAsync_ReturnsViewModel_WhenValid()
        {
            var homeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var landlordId = Guid.NewGuid();
            var home = new Home
            {
                Id = homeId,
                LandlordId = landlordId,
                HomeName = "Test Home",
                HomeLocation = "Test Location",
                HomeType = Data.Data.Models.Enums.HomeType.Apartment,
                HomeDescription = "Test Desc",
                HomeDealType = Data.Data.Models.Enums.DealType.Rent,
                HomePrice = 1000,
                Images = new List<HomeImage>()
            };
            var conversationId = Guid.NewGuid();
            var conversation = new Conversation
            {
                Id = conversationId,
                StartedAt = DateTime.UtcNow,
                UsersConversations = new List<UserConversation>
                {
                    new UserConversation { UserId = userId, User = new User { Id = userId, FullName = "User" } },
                    new UserConversation { UserId = landlordId, User = new User { Id = landlordId, FullName = "Landlord" } }
                }
            };

            _homeRepoMock.Setup(r => r.GetByIdAsync(homeId)).ReturnsAsync(home);
            _convRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new List<Conversation>());
            _convRepoMock.Setup(r => r.AddAsync(It.IsAny<Conversation>())).Returns(Task.CompletedTask);
            _convRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(conversation);

            var result = await _service.GetChatBoxViewModelAsync(homeId, userId);

            result.Should().NotBeNull();
            result.Home.HomeName.Should().Be("Test Home");
            result.OtherParticipantName.Should().Be("Landlord");
        }

        [Test]
        public async Task GetUserConversationsWithDetailsAsync_ReturnsConversations()
        {
            var userId = Guid.NewGuid();
            var conversations = new List<Conversation>
            {
                new Conversation { Id = Guid.NewGuid(), StartedAt = DateTime.UtcNow }
            };
            _convRepoMock.Setup(r => r.GetUserConversationsWithDetailsAsync(userId)).ReturnsAsync(conversations);

            var result = await _service.GetUserConversationsWithDetailsAsync(userId);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }
    }
}
