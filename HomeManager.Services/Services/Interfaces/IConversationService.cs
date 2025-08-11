using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services.Interfaces
{
    public interface IConversationService
    {
        Task<Guid> StartConversationAsync(Guid[] participantIds);
        Task<IEnumerable<ConversationDto>> GetUserConversationsForUserIdAsync(Guid UserId);

        Task<Guid> GetOrCreateConversationForHomeAsync(Guid homeId, Guid userId);
        Task<Guid> GetOrCreateConversationAsync(Guid userId1, Guid userId2);
        Task<Conversation> GetConversationDetailsAsync(Guid conversationId);

        Task<HomeDetailsViewModel> GetChatBoxViewModelAsync(Guid homeId, Guid userId);

        Task<IEnumerable<Conversation>> GetUserConversationsWithDetailsAsync(Guid userId);

    }

}
