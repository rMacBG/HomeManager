using HomeManager.Data.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models.Interfaces
{
    public interface IHome
    {
        public Guid Id { get; }
        public string HomeName { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string  HomeLocation { get; set; }
        public string HomeType { get; set; }
        public string HomeDescription { get; set; }
        public DealType HomeDealType { get; set; }
        public decimal HomePrice { get; set; }
    }
}
