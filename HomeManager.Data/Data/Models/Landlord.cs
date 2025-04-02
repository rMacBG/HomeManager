using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models
{
    public class Landlord : User
    {
        public Landlord(Guid id, string username, string fullName, string phoneNumber)
            : base(id, username, fullName, phoneNumber)
        {
           
        }

        public ICollection<Home> Homes { get; set; } = new List<Home>();

    }
}
