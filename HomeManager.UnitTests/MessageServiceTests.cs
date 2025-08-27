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
    public class MessageServiceTests
    {
        private Mock<IMessageRepository> _msgRepoMock = null!;
        private MessageService _service = null!;

        [SetUp]
        public void Setup()
        {
            _msgRepoMock = new Mock<IMessageRepository>();
            _service = new MessageService(_msgRepoMock.Object);
        }

        [Test]
        public async Task GetMessagesAsync_ReturnsMessages()
        {
            var convId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var messages = new List<Message>
            {
                new Message { Id = Guid.NewGuid(), ConversationId = convId, SenderId = userId, Content = "Hello" }
            };
            
            _msgRepoMock.Setup(r => r.GetByConversationIdAsync(convId)).Returns(Task.FromResult<IEnumerable<Message>>(messages));
            _msgRepoMock.Setup(r => r.GetByConversationIdAsync(convId)).ReturnsAsync(messages);

            var result = await _service.GetMessagesAsync(convId, userId);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Content.Should().Be("Hello");
        }

        [Test]
        public async Task MarkMessagesAsSeenAsync_MarksMessagesAsSeen()
        {
            var convId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var messages = new List<Message>
            {
                new Message { Id = Guid.NewGuid(), ConversationId = convId, ReceiverId = userId, Status = Data.Data.Models.Enums.MessageStatus.Sent }
            };
            _msgRepoMock.Setup(r => r.GetUnseenMessagesAsync(convId, userId)).ReturnsAsync(messages);
            _msgRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Message>())).Returns(Task.CompletedTask);

            await _service.MarkMessagesAsSeenAsync(convId, userId);

            messages[0].Status.Should().Be(Data.Data.Models.Enums.MessageStatus.Seen);
            _msgRepoMock.Verify(r => r.UpdateAsync(messages[0]), Times.Once);
        }
    }
}

