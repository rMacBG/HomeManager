using HomeManager.Data.Data.Dtos;
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

        public async Task<Guid> SendMessageAsync(CreateMessageDto dto)
        {
            var message = new CreateMessageDto
            {
                ConversationId = dto.ConversationId,
                Content = dto.Content,
                SenderId = dto.SenderId,
               
            };
            return message.SenderId;
            
        }
    }
}
