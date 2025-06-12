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
    }
}
