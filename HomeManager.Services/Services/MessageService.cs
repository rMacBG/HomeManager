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
        public async Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid conversationId, Guid currentUserId)
        {
            var messages = await _messageRepository.GetByConversationIdAsync(conversationId);

            return messages.Select(x => new MessageDto
            {
                Id = x.Id,
                Content = x.Content,
                SenderId = x.SenderId,
                ReceiverId = x.ReceiverId,
                MessageStatus = x.SenderId == currentUserId
            ? (Data.Data.Models.Enums.MessageStatus)(int)x.Status
            : (x.Status >= MessageStatus.Delivered
                ? (Data.Data.Models.Enums.MessageStatus)(int)x.Status
                : MessageStatus.Sent),
                SentAt = x.SentAt,
            });

        }

        public async Task MarkMessagesAsSeenAsync(Guid conversationId, Guid userId)
        {
            var messages = await _messageRepository.GetUnseenMessagesAsync(conversationId, userId);
            foreach (var msg in messages)
            {
                msg.Status = MessageStatus.Seen;
                await _messageRepository.UpdateAsync(msg);
            }
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

        public async Task MarkAsDeliveredAsync(Guid messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null) return;

            message.Status = MessageStatus.Delivered;
            await _messageRepository.UpdateAsync(message);
        }

        public async Task<IEnumerable<Message>> MarkAllAsSeenAsync(Guid conversationId, Guid receiverId)
        {
            var messages = await _messageRepository.GetUnseenMessagesAsync(conversationId, receiverId);

            foreach (var msg in messages)
            {
                msg.Status = MessageStatus.Seen;
                await _messageRepository.UpdateAsync(msg);
            }

            return messages;
        }

        public async Task UpdateMessageStatusAsync(Guid messageId, MessageStatus status)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
                throw new Exception("Message not found");

            message.Status = status;
            await _messageRepository.UpdateAsync(message);
           
        }

        public async Task<List<MessageDto>> UpdateMessagesStatusAsync(Guid ConversationId)
        {
            var messages = await _messageRepository.GetByConversationIdAsync(ConversationId);

            var unseenMessages = messages
                .Where(m => m.Status < MessageStatus.Seen)
                .ToList();

            foreach (var message in unseenMessages)
            {
                message.Status = MessageStatus.Seen;
                await _messageRepository.UpdateAsync(message);
            }

            var messagesList = unseenMessages.Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                SentAt = m.SentAt,
                MessageStatus = (Data.Data.Models.Enums.MessageStatus)(int)m.Status
            }).ToList();

            return messagesList;
        }

        public async Task<IEnumerable<MessageDto>> GetUnseenMessagesAsync(Guid conversationId, Guid receiverId)
        {
            var messages = await _messageRepository.GetUnseenMessagesAsync(conversationId, receiverId);
            return messages.Select(m => new MessageDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                SentAt = m.SentAt,
                MessageStatus = m.Status
            });
        }
    }

}
