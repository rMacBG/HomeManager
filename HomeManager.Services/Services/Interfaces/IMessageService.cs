using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Enums;
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
        Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid conversationId, Guid currentUserId);
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task MarkMessagesAsSeenAsync(Guid conversationId, Guid userId);
        Task MarkAsDeliveredAsync(Guid messageId);
        Task UpdateMessageStatusAsync(Guid messageId, MessageStatus status);

        Task<List<MessageDto>> UpdateMessagesStatusAsync(Guid ConversationId);
        Task<IEnumerable<Message>> MarkAllAsSeenAsync(Guid conversationId, Guid receiverId);

    }
}
