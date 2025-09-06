using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Enums;
using HomeManager.Services.Repositories.Interfaces;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public ChatHub(IMessageService messageService, IConversationService conversationService, IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task SendMessage(CreateMessageDto dto)
        {
            try
            {
                var message = await _messageService.SendMessageAsync(dto);

                var payload = new
                {
                    messageId = message.Id,
                    senderId = message.SenderId,
                    receiverId = message.ReceiverId,
                    content = message.Content,
                    sentAt = message.SentAt.ToString("dd/MM/yyyy"),
                    messageStatus = (int)message.MessageStatus,
                    tempId = dto.TempId,
                };

                await Clients.Group(dto.ConversationId.ToString())
                             .SendAsync("ReceiveMessage", payload);


                await Clients.User(message.ReceiverId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        ConversationId = message.ConversationId,
                        SenderName = await GetUserNameAsync(message.SenderId) ?? "Dealer",
                        IsImage = message.Content?.Contains("<img") ?? false,
                        MessageContent = message.Content
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendMessage failed {ex.Message}");
                throw;
            }
        }                                                                                                                                                                  //await Clients.User(message.SenderId.ToString())
                                                                                                                                                                                //    .SendAsync("ReceiveNotification", new {
                                                                                                                                                                                //        ConversationId = message.ConversationId,
                                                                                                                                                                                //        SenderName = await GetUserNameAsync(message.SenderId) ?? "You",
                                                                                                                                                                                //        IsImage = message.Content?.Contains("<img") ?? false,
                                                                                                                                                                                //        MessageContent = message.Content
                                                                                                                                                                                //    });

        
        public async Task AcknowledgeDelivery(Guid messageId)
        {
            await _messageService.MarkAsDeliveredAsync(messageId);

            var message = await _messageService.GetMessageByIdAsync(messageId);

            if (message != null)
            {
                await Clients.Group(message.ConversationId.ToString()).SendAsync("UpdateMessageStatus", new
                {
                    messageId = message.Id,
                    status = (int)message.Status
                });
            }
        }

        public async Task JoinConversationGroup(string conversationId)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, conversationId);
        }
        public async Task MarkAsDelivered(Guid messageId)
        {
            await _messageService.UpdateMessageStatusAsync(messageId, MessageStatus.Delivered);

            var message = await _messageService.GetMessageByIdAsync(messageId);
            if (message != null)
            {
                await Clients.Group(message.ConversationId.ToString()).SendAsync("ReceiveMessageStatusUpdate", new
                {
                    messageId = messageId,
                    status = (int)MessageStatus.Delivered
                });
            }
        }
        public async Task MarkAsSeen(Guid conversationId, Guid userId)
        {
            try
            {
                var unseenMessages = await _messageService.GetUnseenMessagesAsync(conversationId, userId);

                await _messageService.MarkMessagesAsSeenAsync(conversationId, userId);

                foreach (var msg in unseenMessages)
                {
                    await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessageStatusUpdate", new
                    {
                        messageId = msg.Id,
                        status = (int)MessageStatus.Seen
                    });
                }

                await Clients.Group(conversationId.ToString()).SendAsync("ConversationSeen", conversationId, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkAsSeen for conversation {conversationId}: {ex.Message}");
            }
        }

        public async Task SendTyping(string conversationId)
        {
            var userName = Context.User?.Identity?.Name ?? "Someone";
            await Clients.Group(conversationId).SendAsync("ReceiveTyping", userName);
        }

        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.FindFirst("nameidentifier")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                var conversations = await _conversationService.GetUserConversationsForUserIdAsync(userId);
                foreach (var convo in conversations)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, convo.Id.ToString());
                }
            }

            await base.OnConnectedAsync();
        }

        private async Task<string?> GetUserNameAsync(Guid userId)
        {
           
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.Username;
        }
    }
}
