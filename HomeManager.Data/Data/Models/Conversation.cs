using HomeManager.Data.Data.Models.Interfaces;

namespace HomeManager.Data.Data.Models
{
    public class Conversation : IConversation
    {
        public Guid Id { get; }
        public string Title { get; set; }
        public DateTime StartedAt { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<UserConversation> UsersConversations { get; set; }

    }
}