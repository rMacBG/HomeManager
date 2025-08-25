using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public IEnumerable<HomeDto> Estates { get; set; }
    }
}
