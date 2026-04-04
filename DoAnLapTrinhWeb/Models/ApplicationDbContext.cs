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
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<RestaurantTable> Tables { get; set; }
    }
}
