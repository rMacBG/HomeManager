using HomeManager.Data.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Context
{
    public class HomeManagerDbContext : DbContext
    {
        public HomeManagerDbContext()
        {

        }

        public HomeManagerDbContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Home> Homes { get; set; }
        public DbSet<Landlord> Landlords { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserConversation> UsersConversations{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder
                .Entity<UserConversation>(e =>
                e.HasKey(uc => new { uc.UserId, uc.ConversationId }));
            

        }
    }
}
