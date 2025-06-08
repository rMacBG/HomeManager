using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Enums;
using HomeManager.Services.Repositories.Interfaces;
using HomeManager.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid conversationId)
        {
            var messages = await _messageRepository.GetByConversationIdAsync(conversationId);

            return messages.Select(x => new MessageDto
            {
                Id = x.Id,
                Content = x.Content,
                SenderId = x.SenderId,
                ReceiverId = x.ReceiverId,
                MessageStatus = (Data.Data.Models.Enums.MessageStatus)(int)x.Status,
                SentAt = x.SentAt,
            });

        }

        public async Task MarkMessagesAsSeenAsync(Guid conversationId, Guid userId)
        {
            //  var messages = await _messageRepository.GetUnseenMessages;
        }
        public async Task<Message?> GetMessageByIdAsync(Guid messageId)
        {
            return await _messageRepository.GetByIdAsync(messageId);
        }

        public async Task<MessageDto> SendMessageAsync(CreateMessageDto dto)
        {
            var conversation = await _messageRepository.GetConversationWithUsersAsync(dto.ConversationId);

            if (conversation == null)
                throw new InvalidOperationException("Conversation not found.");

            var receiverId = conversation.UsersConversations
                .Where(uc => uc.UserId != dto.SenderId)
                .Select(uc => uc.UserId)
                .FirstOrDefault();

            if (receiverId == Guid.Empty)
                throw new InvalidOperationException("Receiver not found in conversation.");

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = dto.ConversationId,
                Content = dto.Content,
                SenderId = dto.SenderId,
                ReceiverId = receiverId,
                SentAt = dto.SentAt,
                Status = (Data.Data.Models.Enums.MessageStatus)(int)dto.Status,
            };

            await _messageRepository.AddAsync(message);

            message.Status = MessageStatus.Sent;
            await _messageRepository.UpdateAsync(message);
            return new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                Content = message.Content,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                SentAt = message.SentAt,
                MessageStatus = (Data.Data.Models.Enums.MessageStatus)(int)message.Status
            };
        }

    }

}
