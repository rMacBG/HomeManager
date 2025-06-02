using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Dtos
{
    public class ConversationDto
    {
        [Required]
        public Guid Id { get; set; }

        public IEnumerable<Guid> ParticipantsIds { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
