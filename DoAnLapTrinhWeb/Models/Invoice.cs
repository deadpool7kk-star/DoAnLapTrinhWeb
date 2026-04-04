using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnLapTrinhWeb.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        public int? ReservationId { get; set; }
        [ForeignKey("ReservationId")]
        public virtual Reservation? Reservation { get; set; }

        public int? TableId { get; set; }
        [ForeignKey("TableId")]
        public virtual RestaurantTable? Table { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Cancelled

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Cash";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsArchived { get; set; } = false; // Thuộc tính để ẩn hóa đơn khi đã quyết toán tháng mới

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
