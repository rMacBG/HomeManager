﻿using HomeManager.Data.Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace HomeManager.Data.Data.Models
{
    public class Conversation : IConversation
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Title { get; set; }
        [Required]
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Message> Messages { get; set; }
        public ICollection<UserConversation> UsersConversations { get; set; }

    }
}