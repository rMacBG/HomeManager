using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models.Interfaces
{
    public interface IUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
    }
}
