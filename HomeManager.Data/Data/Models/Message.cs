using HomeManager.Data.Data.Models.Enums;
using HomeManager.Data.Data.Models.Interfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models
{
    public class Message : IMessage
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Content { get; set; } 
        [Required]
        public DateTime SentAt { get; set; }
        [Required]
        [Range(0, 4)]
        public MessageStatus Status { get; set; }
        [Required]
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        [Required]
        public Guid SenderId { get; set; }
        public User Sender { get; set; }

        
    }
}
