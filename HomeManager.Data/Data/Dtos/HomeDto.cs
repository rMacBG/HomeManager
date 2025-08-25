using HomeManager.Data.Data.Models;
using HomeManager.Data.Data.Models.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string Region { get; set; }
        public string City { get; set; }
        public HomeType HomeType { get; set; }
        public string HomeDescription { get; set; }
        public DealType HomeDealType { get; set; }
        public decimal HomePrice { get; set; }
        public Guid LandlordId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid ConversationId { get; set; }

        public List<HomeImageDto> Images { get; set; } = new();

    }
}
