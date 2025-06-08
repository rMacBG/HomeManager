using HomeManager.Data.Data.Dtos;
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
                var messageId = await _messageService.SendMessageAsync(dto);

                var message = await _messageService.GetMessageByIdAsync(messageId.Id);

                if (message == null)
                {
                    Console.WriteLine("Message not found after sending.");
                    return;
                }

                var payload = new
                {
                    messageId = message.Id,
                    senderId = message.SenderId,
                    content = message.Content,
                    sentAt = message.SentAt.ToString("dd/MM/yyyy"),
                    status = (int)message.Status
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
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null || message.Status >= MessageStatus.Delivered)
                return;

            message.Status = MessageStatus.Delivered;
            await _messageRepository.UpdateAsync(message);

            await Clients.User(message.SenderId.ToString())
                         .SendAsync("UpdateMessageStatus", new
                         {
                             messageId = message.Id,
                             status = message.Status
                         });
        }
        public async Task MarkMessagesAsSeen(Guid conversationId)
        {
            var userId = Context.UserIdentifier;
            var unseen = await _messageRepository.GetUnseenMessagesAsync(conversationId, Guid.Parse(userId));

            foreach (var msg in unseen)
            {
                msg.Status = MessageStatus.Seen;
                await _messageRepository.UpdateAsync(msg);

                await Clients.User(msg.SenderId.ToString())
                             .SendAsync("UpdateMessageStatus", new
                             {
                                 messageId = msg.Id,
                                 status = msg.Status
                             });
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
