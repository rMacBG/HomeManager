﻿using HomeManager.Data.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Services.Interfaces
{
    public interface IConversationService
    {
        Task<Guid> StartConversationAsync(Guid[] participantIds);
        Task<IEnumerable<ConversationDto>> GetUserConversationsForUserIdAsync(Guid UserId);

    }

}
