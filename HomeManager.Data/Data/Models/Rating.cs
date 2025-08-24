using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models
{
    public class Rating
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid HomeId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Value { get; set; }
        [MaxLength(750)]
        public string? Comment { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
