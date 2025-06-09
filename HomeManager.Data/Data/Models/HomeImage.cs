using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models
{
    public class HomeImage
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FilePath { get; set; }
        public Guid HomeId { get; set; }
        public Home Home { get; set; }
    }
}
