using HomeManager.Data.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Dtos
{
    public class CreateMessageDto
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = null!;

        public Guid ReceiverId { get; set; }
        public DateTime SentAt { get; set; }

        public MessageStatus Status { get; set; }

        public string TempId { get; set; }

    }
}
