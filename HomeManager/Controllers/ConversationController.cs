using HomeManager.Data.Data.Models;
using HomeManager.Services.Services;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeManager.Controllers
{
    public class ConversationController : Controller
    {
        
        private readonly IMessageService _messageService;

        public ConversationController( IMessageService messageService)
        {
                 _messageService = messageService;
        }
        [HttpGet]
        [Authorize]
        public async Task<JsonResult> GetMessages(Guid conversationId)
        {
            var messages = await _messageService.GetMessagesAsync(conversationId);
           // var conversation = await _conversationService.GetOrCreateConversationForHomeAsync(homeId, Guid.Parse(userId));
            return Json(messages);
        }
    }
}
