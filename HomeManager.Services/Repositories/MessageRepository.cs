using HomeManager.Data.Data.Context;
using HomeManager.Data.Data.Models;
using HomeManager.Services.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly HomeManagerDbContext _context;
        public MessageRepository(HomeManagerDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Message message)
        {
           await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetByConversationIdAsync(Guid conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
