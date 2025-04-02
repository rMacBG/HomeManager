using HomeManager.Data.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models.Interfaces
{
    public interface IUser : IGuidId
    { 
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public Role Role { get; set; }
    }
}
