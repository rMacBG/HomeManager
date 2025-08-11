using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Enums;
using HomeManager.Data.Data.ViewModels;
using HomeManager.Services.Services;
using HomeManager.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeManager.Controllers
{
    [Route("Chat")]
    public class ConversationController : Controller
    {

        private readonly IMessageService _messageService;
        private readonly IHomeService _homeService;
        private readonly IConversationService _conversationService;

        public ConversationController(IMessageService messageService, IHomeService homeService, IConversationService conversationService)
        {
            _messageService = messageService;
            _homeService = homeService;
            _conversationService = conversationService;
        }

        [Authorize]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
            {
                return Forbid();
            }
            Guid userId = Guid.Parse(userIdStr);

            var conversations = await _conversationService.GetUserConversationsWithDetailsAsync(userId);


            var viewModel = conversations.Select(c =>
            {
                var lastMessage = c.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault();
                return new ConversationListItemViewModel
                {
                    ConversationId = c.Id,
                    OtherParticipantName = c.UsersConversations?
                        .FirstOrDefault(uc => uc.UserId != userId)?.User?.FullName
                        ?? c.UsersConversations?.FirstOrDefault(uc => uc.UserId != userId)?.User?.Username
                        ?? "Unknown",
                    CreatedAt = c.StartedAt,
                    HomeName = c.Home?.HomeName ?? "Unknown Home",
                    HomeImageUrl = c.Home?.Images?.FirstOrDefault()?.FilePath ?? "/images/default-home.png",
                    UnreadCount = c.Messages?.Count(m => m.ReceiverId == userId && (int)m.Status < (int)MessageStatus.Seen) ?? 0,
                    LastMessagePreview = lastMessage != null ? lastMessage.Content : ""
                };
            }).OrderByDescending(x => x.CreatedAt).ToList();

            return View("~/Views/Chat/Index.cshtml", viewModel);
        }
        [HttpGet("Messages")]
        [Authorize]
        public async Task<JsonResult> GetMessages(Guid conversationId, Guid currentUserId)
        {
            var messages = await _messageService.GetMessagesAsync(conversationId, currentUserId);
            return Json(messages);
        }
        [HttpGet("Conversation/{id}")]
        [Authorize]
        public async Task<IActionResult> Conversation(Guid id)
        {
            var conversation = await _conversationService.GetConversationDetailsAsync(id);
            if (conversation == null)
            {
                return NotFound();
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid currentUserId = Guid.Parse(userIdStr);
            var otherParticipantId = conversation.UsersConversations.FirstOrDefault(u => u.UserId != currentUserId)?.UserId;

            var otherUser = conversation.UsersConversations
                .FirstOrDefault(uc => uc.UserId != currentUserId)?.User;

            var viewModel = new ConversationDetailsViewModel
            {
                ConversationId = conversation.Id,
                Messages = conversation.Messages.OrderBy(m => m.SentAt).ToList(),
                CurrentUserId = currentUserId,
                OtherParticipantName = otherUser?.FullName ?? otherUser?.Username ?? "Unknown"
            };

            return View("~/Views/Chat/Conversation.cshtml", viewModel);
        }
        [Authorize]
        [HttpGet("ForHome/{homeId}")]
        public async Task<IActionResult> GetOrCreateConversation(Guid homeId)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdStr == null) return Unauthorized();

                var userId = Guid.Parse(userIdStr);

                var conversationId = await _conversationService.GetOrCreateConversationForHomeAsync(homeId, userId);

                var conversation = await _conversationService.GetConversationDetailsAsync(conversationId);
                if (conversation == null)
                {
                    return NotFound("Conversation not found.");
                }

                var otherUser = conversation.UsersConversations
                        .FirstOrDefault(uc => uc.UserId != userId)?.User;

                var otherUserName = otherUser?.FullName ?? otherUser?.Username ?? "Unknown";

                return Ok(new
                {
                    conversationId,
                    otherUserName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }

        [HttpGet("api/conversations/user")]
        [Authorize]
        public async Task<IActionResult> GetUserConversations()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
            {
                return Unauthorized();
            }

            Guid userId = Guid.Parse(userIdStr);
            var conversations = await _conversationService.GetUserConversationsForUserIdAsync(userId);


            return Ok(conversations);
        }
        [HttpGet("ChatBoxPartial")]
        [Authorize]
        public async Task<IActionResult> ChatBoxPartial(Guid conversationId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null) return Unauthorized();

            var conversation = await _conversationService.GetConversationDetailsAsync(conversationId);
            if (conversation == null) return NotFound();

            var otherUser = conversation.UsersConversations
                .FirstOrDefault(uc => uc.UserId != Guid.Parse(userIdStr))?.User;

            var viewModel = new HomeDetailsViewModel
            {
                Home = null,
                Conversation = new ConversationDto
                {
                    Id = conversation.Id,
                    ParticipantsIds = conversation.UsersConversations.Select(uc => uc.UserId),
                    CreatedAt = conversation.StartedAt
                },
                OtherParticipantName = otherUser?.FullName ?? otherUser?.Username ?? "Unknown",
                Messages = conversation.Messages.Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    MessageStatus = m.Status,
                })
            };

            return PartialView("_ChatBox", viewModel);
        }

        [HttpPost("UploadFile")]
        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile file, Guid conversationId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Generate a unique filename and save to wwwroot/uploads
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/uploads/{fileName}";

            // Optionally: Save a message with this file in the conversation (if you want to show it immediately)
            // await _messageService.AddFileMessageAsync(conversationId, url, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return Json(new { url });
        }
    }
}
