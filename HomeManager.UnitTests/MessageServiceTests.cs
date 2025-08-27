using FluentAssertions;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Enums;
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
        [Test]
        public async Task GetMessageByIdAsync_ReturnsMessage_WhenFound()
        {
            var msgId = Guid.NewGuid();
            var message = new Message { Id = msgId, Content = "Test" };
            _msgRepoMock.Setup(r => r.GetByIdAsync(msgId)).ReturnsAsync(message);

            var result = await _service.GetMessageByIdAsync(msgId);

            result.Should().NotBeNull();
            result!.Content.Should().Be("Test");
        }

        [Test]
        public async Task GetMessageByIdAsync_ReturnsNull_WhenNotFound()
        {
            var msgId = Guid.NewGuid();
            _msgRepoMock.Setup(r => r.GetByIdAsync(msgId)).ReturnsAsync((Message?)null);

            var result = await _service.GetMessageByIdAsync(msgId);

            result.Should().BeNull();
        }

        [Test]
        public async Task MarkAsDeliveredAsync_UpdatesStatus_WhenMessageExists()
        {
            var msgId = Guid.NewGuid();
            var message = new Message { Id = msgId, Status = MessageStatus.Sent };
            _msgRepoMock.Setup(r => r.GetByIdAsync(msgId)).ReturnsAsync(message);
            _msgRepoMock.Setup(r => r.UpdateAsync(message)).Returns(Task.CompletedTask);

            await _service.MarkAsDeliveredAsync(msgId);

            message.Status.Should().Be(MessageStatus.Delivered);
            _msgRepoMock.Verify(r => r.UpdateAsync(message), Times.Once);
        }

        [Test]
        public async Task MarkAsDeliveredAsync_DoesNothing_WhenMessageNotFound()
        {
            var msgId = Guid.NewGuid();
            _msgRepoMock.Setup(r => r.GetByIdAsync(msgId)).ReturnsAsync((Message?)null);

            await _service.MarkAsDeliveredAsync(msgId);

            _msgRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public async Task UpdateMessageStatusAsync_UpdatesStatus_WhenMessageExists()
        {
            var msgId = Guid.NewGuid();
            var message = new Message { Id = msgId, Status = MessageStatus.Sent };
            _msgRepoMock.Setup(r => r.GetByIdAsync(msgId)).ReturnsAsync(message);
            _msgRepoMock.Setup(r => r.UpdateAsync(message)).Returns(Task.CompletedTask);

            await _service.UpdateMessageStatusAsync(msgId, MessageStatus.Seen);

            message.Status.Should().Be(MessageStatus.Seen);
            _msgRepoMock.Verify(r => r.UpdateAsync(message), Times.Once);
        }

        [Test]
        public void UpdateMessageStatusAsync_Throws_WhenMessageNotFound()
        {
            var msgId = Guid.NewGuid();
            _msgRepoMock.Setup(r => r.GetByIdAsync(msgId)).ReturnsAsync((Message?)null);

            Func<Task> act = async () => await _service.UpdateMessageStatusAsync(msgId, MessageStatus.Seen);

            act.Should().ThrowAsync<Exception>().WithMessage("Message not found");
        }

        [Test]
        public async Task GetUnseenMessagesAsync_ReturnsUnseenMessages()
        {
            var convId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var messages = new List<Message>
            {
                new Message { Id = Guid.NewGuid(), ConversationId = convId, ReceiverId = receiverId, Status = MessageStatus.Sent, Content = "Unseen" }
            };
            _msgRepoMock.Setup(r => r.GetUnseenMessagesAsync(convId, receiverId)).ReturnsAsync(messages);

            var result = await _service.GetUnseenMessagesAsync(convId, receiverId);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Content.Should().Be("Unseen");
        }
    }
}

