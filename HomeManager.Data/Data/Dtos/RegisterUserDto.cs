using HomeManager.Data.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Dtos
{
    public class RegisterUserDto
    {
        [Required]
        [Length(3, 20)]
        public string Username { get; set; }
        [Required]
        [Length(2, 80)]
        public string FullName { get; set; }

        [Required]
        [Length(7, 15, ErrorMessage = "Password must be between 7 and 15 characters long!")]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^(\+\d{1,3}\s?)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$", ErrorMessage ="Invalid phone format!")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        public Role Role { get; set; }
    }
}
