using HomeManager.Data.Data.Models;
using HomeManager.Services.Services;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeManager.Controllers
{
    public class ConversationController : Controller
    {
        
        private readonly IMessageService _messageService;
        private readonly IHomeService _homeService;
        private readonly IConversationService _conversationService;

        public ConversationController( IMessageService messageService, IHomeService homeService, IConversationService conversationService)
        {
                 _messageService = messageService;
                 _homeService = homeService;
                 _conversationService = conversationService;
        }
        [HttpGet]
        [Authorize]
        public async Task<JsonResult> GetMessages(Guid conversationId)
        {
            var messages = await _messageService.GetMessagesAsync(conversationId);
            return Json(messages);
        }
        [Authorize]
        [HttpGet("api/conversations/for-home/{homeId}")]
        public async Task<IActionResult> GetOrCreateConversation(Guid homeId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized();

                var conversationId = await _conversationService.GetOrCreateConversationForHomeAsync(homeId, Guid.Parse(userId));

                return Ok(new { conversationId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });

            }

        }
    }
}
