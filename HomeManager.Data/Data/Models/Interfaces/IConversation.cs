using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models.Interfaces
{
    public interface IConversation : IGuidId
    {
        public string Title { get; set; }
        public DateTime StartedAt { get; set; }
        public ICollection<Message> Messages { get; set; }

        public ICollection<UserConversation> UsersConversations { get; set; }
    }
}
