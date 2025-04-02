using HomeManager.Data.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models
{
    public class Landlord : User
    {
        public Landlord(Guid id, string username, string fullName, string phoneNumber, Role role)
            : base(id, username, fullName, phoneNumber, role)
        {
           
        }
        [Required]
        public string Residence { get; set; }
        public ICollection<Home> Homes { get; set; } = new List<Home>();

    }
}
