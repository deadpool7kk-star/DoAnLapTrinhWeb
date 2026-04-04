using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnLapTrinhWeb.Models
{
    public class Dish
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên món")]
        [StringLength(200)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        [StringLength(50)]
        public string? Badge { get; set; } // Ví dụ: Mới, Signature, Phổ Biến...

        [StringLength(50)]
        public string? Icon { get; set; } // Ví dụ: 🥩, 🐟, 🍝

        public bool IsVisible { get; set; } = true;

        public int? DiscountPercentage { get; set; } // Ví dụ: 10, 20... (phần trăm giảm)

        [NotMapped]
        public decimal DiscountedPrice => DiscountPercentage.HasValue 
            ? Price * (1 - (decimal)DiscountPercentage.Value / 100) 
            : Price;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
