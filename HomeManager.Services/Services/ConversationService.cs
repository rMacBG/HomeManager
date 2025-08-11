using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.ViewModels;
using HomeManager.Services.Repositories;
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
        private readonly IHomeRepository _homeRepository;

        public ConversationService(IUserRepository userRepository, IConversationRepository conversationRepository, IHomeRepository homeRepository)
        {
            _userRepository = userRepository;
            _conversationRepository = conversationRepository;
            _homeRepository = homeRepository;
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
        public async Task<Guid> GetOrCreateConversationForHomeAsync(Guid homeId, Guid userId)
        {
            var home = await _homeRepository.GetByIdAsync(homeId);

            Console.WriteLine($"Home ID: {homeId}");
            Console.WriteLine($"Logged-in User ID: {userId}");
            Console.WriteLine($"Home Landlord ID: {home.LandlordId}");

            //if (home == null || home.LandlordId == userId)
            //    throw new InvalidOperationException("Invalid home or cannot start a conversation with yourself.");
            if (userId == home.LandlordId)
            {
                throw new InvalidOperationException("Cannot send messages to yourself.");
            }

            return await GetOrCreateConversationAsync(userId, home.LandlordId);
        }

        public async Task<Guid> GetOrCreateConversationAsync(Guid userId1, Guid userId2)
        {
            try
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
                    Title = $"{userId1}'s chat",
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
            catch (Exception ex)
            {
                Console.WriteLine("Error creating convo: " + ex);
                throw;
            }
        }

        public async Task<Conversation> GetConversationDetailsAsync(Guid conversationId)
        {
            var conversation = await _conversationRepository.GetByIdAsync(conversationId);
            if (conversation == null)
            {
                throw new Exception("Conversation not found");
            }
            return conversation;
        }

        public async Task<HomeDetailsViewModel> GetChatBoxViewModelAsync(Guid homeId, Guid userId)
        {
            var home = await _homeRepository.GetByIdAsync(homeId);
            if (home == null)
                return null;

            if (home.LandlordId == userId)
                throw new InvalidOperationException("Cannot start a conversation with yourself.");

            var conversationId = await GetOrCreateConversationAsync(userId, home.LandlordId);
            var conversation = await GetConversationDetailsAsync(conversationId);

            var conversationDto = new ConversationDto
            {
                Id = conversation.Id,
                ParticipantsIds = conversation.UsersConversations.Select(uc => uc.User.Id),
                CreatedAt = conversation.StartedAt
            };

            var homeDto = new HomeDto
            {
                Id = home.Id,
                HomeName = home.HomeName,
                HomeLocation = home.HomeLocation,
                HomeType = home.HomeType,
                HomeDescription = home.HomeDescription,
                HomeDealType = home.HomeDealType,
                HomePrice = home.HomePrice,
                LandlordId = home.LandlordId,
                ConversationId = conversation.Id,
                Images = home.Images?.Select(img => new HomeImageDto
                {
                    FilePath = img.FilePath,
                }).ToList() ?? new List<HomeImageDto>()
            };

            var otherUser = conversation.UsersConversations
                .Select(uc => uc.User)
                .FirstOrDefault(u => u.Id != userId);

            var otherParticipantName = otherUser?.FullName ?? otherUser?.Username ?? "Dealer";

            return new HomeDetailsViewModel
            {
                Home = homeDto,
                Conversation = conversationDto,
                OtherParticipantName = otherParticipantName
            };
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsWithDetailsAsync(Guid userId)
        {
            return await _conversationRepository.GetUserConversationsWithDetailsAsync(userId);
        }
    }
}
