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

        public ChatHub(IMessageService messageService, IConversationService conversationService, IMessageRepository messageRepository)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _messageRepository = messageRepository;
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
                    content = message.Content,
                    sentAt = message.SentAt.ToString("dd/MM/yyyy"),
                    messageStatus = (int)message.MessageStatus,
                    tempId = dto.TempId,
                };

                await Clients.Group(dto.ConversationId.ToString())
                             .SendAsync("ReceiveMessage", payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendMessage failed {ex.Message}");
                throw;
            }
        }
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
                await _messageService.MarkMessagesAsSeenAsync(conversationId, userId);

                var updatedMessages = await _messageService.GetUnseenMessagesAsync(conversationId, userId);
                foreach (var msg in updatedMessages)
                {
                    await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessageStatusUpdate", new
                    {
                        messageId = msg.Id,
                        status = (int)MessageStatus.Seen
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkAsSeen for conversation {conversationId}: {ex.Message}");
            }
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
    }
}
