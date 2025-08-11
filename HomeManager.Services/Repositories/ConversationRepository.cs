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
    public class ConversationRepository : IConversationRepository
    {
        private readonly HomeManagerDbContext _context;
        public ConversationRepository(HomeManagerDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task<Conversation?> GetByIdAsync(Guid id)
        {
            return await _context.Conversations
                .Include(c => c.UsersConversations)
                .ThenInclude(uc => uc.User)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Conversation>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Conversations
                .Where(x => x.UsersConversations.Any(uc => uc.UserId == userId))
                .Include(x => x.UsersConversations)
                .ToListAsync();
        }
 
    }
}
