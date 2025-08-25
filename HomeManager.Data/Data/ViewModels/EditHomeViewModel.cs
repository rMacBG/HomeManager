using HomeManager.Data.Data.Models.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.ViewModels
{
    public class EditHomeViewModel
    {
        public Guid Id { get; set; }
        public string HomeName { get; set; }
        public string HomeDescription { get; set; }
        public string HomeLocation { get; set; }
        public decimal HomePrice { get; set; }
        public string Region { get; set; }
        public string City { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public HomeType HomeType { get; set; }
        public DealType HomeDealType { get; set; }
        public List<IFormFile> UploadedImages { get; set; } = new();
        public List<string> ExistingImages { get; set; } = new();
        public List<string> ImagesToRemove { get; set; } = new(); 
    }
}
