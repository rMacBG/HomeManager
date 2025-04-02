using HomeManager.Data.Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeManager.Data.Data.Models
{
    public class UserConversation : IUserConversation
    {
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        [ForeignKey(nameof(Conversation))]
        public int ConversaionId { get; set; }
        
        public Conversation Conversation { get; set; }
    }
}