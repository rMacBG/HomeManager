using HomeManager.Data.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Dtos
{
    public class RegisterUserDto
    {
        public string Username { get; set; }
        public string FullName { get; set; }

        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public Role Role { get; set; }
    }
}
