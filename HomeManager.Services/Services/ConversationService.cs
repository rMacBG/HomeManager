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
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserRepository _userRepository;
        public ConversationService(IUserRepository userRepository, IConversationRepository conversationRepository)
        {
            _userRepository = userRepository;
            _conversationRepository = conversationRepository;
        }

        public async Task<IEnumerable<ConversationDto>> GetUserConversationsForUserIdAsync(Guid UserId)
        {
            var conversations = await _conversationRepository.GetByUserIdAsync(UserId);

            return conversations.Select(c => new ConversationDto
            {
                Id = c.Id,
                ParticipantsIds = c.UsersConversations.Select(uc => uc.UserId).ToList(),
                CreatedAt = c.StartedAt,
            });
        }

        public async Task<Guid> StartConversationAsync(Guid[] participantIds)
        {
            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                StartedAt = DateTime.UtcNow,
                UsersConversations = participantIds.Select(pid => new UserConversation
                {
                    UserId = pid,
                }).ToList()
            };
            await _conversationRepository.AddAsync(conversation);
            return conversation.Id;
        }
        //public async Task<Guid> GetOrCreateConversationAsync(Guid homeId, Guid userId)
        //{
        //    var conversation = await _conversationRepository.FindByHomeAndUserAsync(homeId, userId);
        //    if (conversation != null) return conversation.Id;

        //    var newConv = new Conversation
        //    {
        //        Id = Guid.NewGuid(),
        //        HomeId = homeId,
        //        CreatedAt = DateTime.UtcNow,
        //        Participants = new List<UserConversation> {
        //    new UserConversation { UserId = userId }
        //}
        //    };

        //    await _conversationRepository.AddAsync(newConv);
        //    return newConv.Id;
        //}
        public async Task<Guid> GetOrCreateConversationAsync(Guid userId1, Guid userId2)
        {
            var allConversations = await _conversationRepository.GetByUserIdAsync(userId1);


            var existingConversation = allConversations
                .FirstOrDefault(conv =>
                    conv.UsersConversations.Any(uc => uc.UserId == userId2) &&
                    conv.UsersConversations.Count == 2);

            if (existingConversation != null)
                return existingConversation.Id;


            var newConversation = new Conversation
            {
                Id = Guid.NewGuid(),
                StartedAt = DateTime.UtcNow,
                UsersConversations = new List<UserConversation>
        {
            new UserConversation { UserId = userId1 },
            new UserConversation { UserId = userId2 }
        }
            };

            await _conversationRepository.AddAsync(newConversation);
            return newConversation.Id;
        }
    }
}
