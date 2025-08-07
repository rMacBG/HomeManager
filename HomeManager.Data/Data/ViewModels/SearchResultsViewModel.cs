using HomeManager.Data.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.ViewModels
{
    public class SearchResultsViewModel
    {
        public string Query { get; set; }
        public List<HomeDto> HomeResults { get; set; }
        public List<UserDto> UserResults { get; set; }
    }
}
