using HomeManager.Data.Data.Models.Enums;
using HomeManager.Data.Data.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models
{
    public class User : IUser
    {
        
        [Key]
        public Guid Id { get ; set; }
        [Required]
        [MaxLength(20)]
        public string Username { get; set; } = null!;
        [Required]
        [MaxLength(75)]
        public string FullName { get; set; } = null!;

        //[Required]
        [MaxLength(120)]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; }
        [Required]
        [MaxLength(25)]
        public string PhoneNumber { get; set; } = null!;


        public string? Bio { get; set; }

        public string? AvatarUrl { get; set; }
        public Role Role { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        public ICollection<UserConversation> UsersConversations { get; set; }

        //public ICollection<Home> BookmarkedHomes { get; set; } = new List<Home>();

    }
}
