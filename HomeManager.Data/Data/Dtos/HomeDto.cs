using HomeManager.Data.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Dtos
{
    public class HomeDto
    {
        public Guid Id { get; set; }
        public string HomeName { get; set; }
        public string HomeLocation { get; set; }
        public string HomeType { get; set; }
        public string HomeDescription { get; set; }
        public DealType HomeDealType { get; set; }
        public decimal HomePrice { get; set; }
        public Guid LandlordId { get; set; }
    }
}
