using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services.Interfaces
{
    public interface IMessageService
    {
        Task<MessageDto> SendMessageAsync(CreateMessageDto dto);
        Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid conversationId);
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task MarkMessagesAsSeenAsync(Guid conversationId, Guid userId);
        
    }
}
