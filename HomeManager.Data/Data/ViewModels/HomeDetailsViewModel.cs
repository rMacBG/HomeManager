using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.ViewModels
{
    public class HomeDetailsViewModel
    {
        public HomeDto Home { get; set; }

        public string OwnerName { get; set; }
        public ConversationDto Conversation { get; set; }
        public string DealerPhone { get; set; }
        public List<Rating> Ratings { get; set; } = new();
        public double AverageRating => Ratings.Any() ? Ratings.Average(r => r.Value) : 0;
        public string LandlordAvatarUrl { get; set; }
        public string OtherParticipantName { get; set; }
        public IEnumerable<MessageDto> Messages { get; set; }
    }
}
