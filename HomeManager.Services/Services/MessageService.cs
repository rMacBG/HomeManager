using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
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
                MessageStatus = x.Status,
                SentAt = x.SentAt,
            });

        }

        public async Task MarkMessagesAsSeenAsync(Guid conversationId, Guid userId)
        {
          //  var messages = await _messageRepository.GetUnseenMessages;
        }

        public async Task<MessageDto> SendMessageAsync(CreateMessageDto dto)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = dto.ConversationId,
                Content = dto.Content,
                SenderId = dto.SenderId,
                SentAt = dto.SentAt,
                Status = (Data.Data.Models.Enums.MessageStatus)(int)dto.Status,
               
            };

            await _messageRepository.AddAsync(message);



            return new MessageDto {
                Id = message.Id,
                ConversationId = message.ConversationId,
                Content = message.Content,
                SenderId = message.SenderId,
                SentAt = message.SentAt,
                MessageStatus = message.Status
            };


        }
    }
}
