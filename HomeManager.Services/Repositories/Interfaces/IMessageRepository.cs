using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Repositories.Interfaces
{
    public interface IMessageRepository 
    {
        Task AddAsync(Message message);
        Task<IEnumerable<Message>> GetByConversationIdAsync(Guid conversationId);
    }
}
