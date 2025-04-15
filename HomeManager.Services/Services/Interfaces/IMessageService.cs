using HomeManager.Data.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services.Interfaces
{
    public interface IMessageService
    {
        Task<Guid> SendMessageAsync(CreateMessageDto dto);
        Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid conversationId);
    }
}
