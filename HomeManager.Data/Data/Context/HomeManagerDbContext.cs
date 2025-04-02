using HomeManager.Data.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
    public class HomeManagerDbContext : IdentityDbContext
    {
        public HomeManagerDbContext()
        {

        }

        public HomeManagerDbContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UserConversation>(e =>
                e.HasKey(uc => new { uc.UserId, uc.ConversationId }));

        }
    }
}
