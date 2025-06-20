﻿using HomeManager.Data.Data.Models.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Dtos
{
    public class CreateHomeDto
    {
        [Required]
        [MinLength(3)]
        public string HomeName { get; set; }
        [Required]
        [MinLength(3)]
        public string HomeLocation { get; set; }
        [Required]
        public HomeType HomeType { get; set; }
        [Required]
        [MinLength(3)]
        public string HomeDescription { get; set; }
        [Required(ErrorMessage = "Please select a deal type.")]
        public DealType HomeDealType { get; set; }
        [Required]
        public decimal HomePrice { get; set; }
        [Required]
        public Guid LandlordId { get; set; }

        public List<IFormFile> UploadedImages { get; set; } = new();
    }
}
