using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Repositories.Interfaces
{
    public interface IConversationRepository
    {
        Task AddAsync(Conversation conversation);

        Task<Conversation?> GetByIdAsync(Guid id);

        Task<IEnumerable<Conversation>> GetByUserIdAsync(Guid userId);

        Task<IEnumerable<Conversation>> GetUserConversationsWithDetailsAsync(Guid userId);
    }
}
