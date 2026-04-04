using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoAnLapTrinhWeb.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } // Danh sách danh mục món ăn
        public DbSet<Dish> Dishes { get; set; } // Danh sách món ăn
        public DbSet<Reservation> Reservations { get; set; } // Danh sách đặt bàn
        public DbSet<RestaurantTable> Tables { get; set; } // Danh sách bàn ăn
        public DbSet<Invoice> Invoices { get; set; } // Danh sách hóa đơn
        public DbSet<InvoiceItem> InvoiceItems { get; set; } // Danh sách chi tiết hóa đơn
    }
}
