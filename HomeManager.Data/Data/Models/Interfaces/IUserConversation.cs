using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models.Interfaces
{
    public interface IUserConversation
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int ConversaionId { get; set; }
        public Conversation Conversation { get; set; }
    }
}
