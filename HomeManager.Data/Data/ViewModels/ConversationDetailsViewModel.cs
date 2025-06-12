using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.ViewModels
{
    public class ConversationDetailsViewModel
    {
        public Guid ConversationId { get; set; }
        public string OtherParticipantName { get; set; }
        public Guid CurrentUserId { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }
}
