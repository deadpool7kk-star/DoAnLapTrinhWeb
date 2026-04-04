using System.ComponentModel.DataAnnotations;

namespace DoAnLapTrinhWeb.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ")]
        [DataType(DataType.Time)]
        public string BookingTime { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số khách")]
        public string GuestCount { get; set; }

        public string Note { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

        public string? UserId { get; set; }
        public int? TableId { get; set; }
        public virtual RestaurantTable? Table { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
