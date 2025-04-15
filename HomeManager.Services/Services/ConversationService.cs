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
    }
}
