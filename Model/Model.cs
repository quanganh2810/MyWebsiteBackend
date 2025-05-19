using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodApi.Models
{
    public class Destination
    {
        [Key]
        public string? Item { get; set; }
        public string? Address { get; set; }
        public string? Place { get; set; }
        public string? Map { get; set; }
    }
}
