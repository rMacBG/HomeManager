using HomeManager.Data.Data.Dtos;
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

        public ConversationDto Conversation { get; set; }

        public IEnumerable<MessageDto> Messages { get; set; }
    }
}
