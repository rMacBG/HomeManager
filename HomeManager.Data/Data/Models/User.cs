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
        public User(Guid id, string username, string passwordHash,string fullName, string phoneNumber, Role role)
        {
            this.Id = id;
            this.Username = username;
            this.PasswordHash = passwordHash;
            this.FullName = fullName;
            this.PhoneNumber = phoneNumber;
            this.Role = role;
        }
        [Key]
        public Guid Id { get ; set; }
        [Required]
        [MaxLength(20)]
        public string Username { get; set; } = null!;
        [Required]
        [MaxLength(75)]
        public string FullName { get; set; } = null!;
        [Required]
        [MaxLength(20)]
        public string PasswordHash { get; set; }
        [Required]
        [MaxLength(13)]
        public string PhoneNumber { get; set; } = null!;
        public Role Role { get; set; }
    }
}
