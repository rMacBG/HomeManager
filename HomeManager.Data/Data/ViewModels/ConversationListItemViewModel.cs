using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.ViewModels
{
    public class ConversationListItemViewModel
    {
        public Guid ConversationId { get; set; }
        public string OtherParticipantName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string HomeName { get; set; }
        public string HomeImageUrl { get; set; }
        public int UnreadCount { get; set; }
        public string LastMessagePreview { get; set; }
    }
}
