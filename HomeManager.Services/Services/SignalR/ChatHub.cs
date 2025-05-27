using HomeManager.Data.Data.Dtos;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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

        public ChatHub(IMessageService messageService, IConversationService conversationService)
        {
            _messageService = messageService;
            _conversationService = conversationService;
        }

        public async Task SendMessage(CreateMessageDto dto)
        {
            try
            {


                var messageId = await _messageService.SendMessageAsync(dto);

                await Clients.Group(dto.ConversationId.ToString()).SendAsync("ReceiveMessage", new
                {
                    MessageId = messageId,
                    dto.SenderId,
                    dto.Content,
                    SentAt = DateTime.UtcNow.ToString("dd/MM/yyyy")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendMessage failed {ex.Message}");
                throw;
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
