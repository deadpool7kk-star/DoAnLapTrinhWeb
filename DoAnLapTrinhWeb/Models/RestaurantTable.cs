using System.ComponentModel.DataAnnotations;

namespace DoAnLapTrinhWeb.Models
{
    public class RestaurantTable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public int Capacity { get; set; } // 2, 4, 6...

        [Required]
        public int Floor { get; set; } // 1 or 2

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Available"; // Available, Occupied, Reserved

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
