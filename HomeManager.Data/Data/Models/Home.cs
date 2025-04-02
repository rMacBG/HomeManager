using HomeManager.Data.Data.Models.Enums;
using HomeManager.Data.Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace HomeManager.Data.Data.Models
{
    public class Home : IHome
    {
        [Key]
        public Guid Id { get; }

        [Required]
        [MaxLength(100)]
        public string HomeName { get; set; } = null!;
        [Required]
        public DateTime AddedAt { get; set; }
        [Required]
        public DateTime LastModifiedAt { get; set; }
        [Required]
        [MaxLength(100)]
        public string HomeLocation { get; set; } = null!;
        [Required]
        public string HomeType { get; set; } = null!;
        [Required]
        [MaxLength(1000)]
        public string HomeDescription { get; set; } = null!;
        [Required]
        public DealType HomeDealType { get; set; } 
        [Required]
        [Range(0.01, 100000000, ConvertValueInInvariantCulture = true)]
        public decimal HomePrice { get; set; } 
    }
}