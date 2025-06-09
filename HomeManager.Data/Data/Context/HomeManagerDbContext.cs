using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Enums;
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

        public DbSet<HomeImage> HomeImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var adminUser = new User
            {
                Id = adminId,
                Username = "admin",
                FullName = "Admin User",
                PhoneNumber = "0000000000",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = Role.Admin
            };
            modelBuilder.Entity<User>().HasData(adminUser);

            modelBuilder
                .Entity<UserConversation>(e =>
                e.HasKey(uc => new { uc.UserId, uc.ConversationId }));

            modelBuilder.Entity<UserConversation>()
    .HasOne(uc => uc.User)
    .WithMany(u => u.UsersConversations)
    .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserConversation>()
                .HasOne(uc => uc.Conversation)
                .WithMany(c => c.UsersConversations)
                .HasForeignKey(uc => uc.ConversationId);

            modelBuilder.Entity<Message>()
        .HasOne(m => m.Sender)
        .WithMany()
        .HasForeignKey(m => m.SenderId)
        .OnDelete(DeleteBehavior.Restrict); // <== prevent cascade

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
