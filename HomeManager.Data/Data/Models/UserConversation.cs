using HomeManager.Data.Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeManager.Data.Data.Models
{
    public class UserConversation : IUserConversation
    {
      
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        public Guid ConversationId { get; set; }
        
        public Conversation Conversation { get; set; }
        
    }
}