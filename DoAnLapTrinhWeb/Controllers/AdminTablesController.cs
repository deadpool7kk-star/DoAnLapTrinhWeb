using Microsoft.AspNetCore.Mvc;
using DoAnLapTrinhWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAnLapTrinhWeb.Controllers
{
    public class AdminTablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminTablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Seed tables if empty
            if (!await _context.Tables.AnyAsync())
            {
                await SeedTables();
            }

            var tables = await _context.Tables.OrderBy(t => t.Floor).ThenBy(t => t.Id).ToListAsync();
            return View(tables);
        }

        private async Task SeedTables()
        {
            var tables = new List<RestaurantTable>();

            // Floor 1 (10 tables)
            // 4 x (1-2), 4 x (3-4), 2 x (5+)
            for (int i = 1; i <= 4; i++) tables.Add(new RestaurantTable { Name = $"Bàn {i:D2}", Capacity = 2, Floor = 1 });
            for (int i = 5; i <= 8; i++) tables.Add(new RestaurantTable { Name = $"Bàn {i:D2}", Capacity = 4, Floor = 1 });
            for (int i = 9; i <= 10; i++) tables.Add(new RestaurantTable { Name = $"Bàn {i:D2}", Capacity = 6, Floor = 1 });

            // Floor 2 (10 tables)
            // 4 x (1-2), 3 x (3-4), 3 x (5+)
            for (int i = 11; i <= 14; i++) tables.Add(new RestaurantTable { Name = $"Bàn {i:D2}", Capacity = 2, Floor = 2 });
            for (int i = 15; i <= 17; i++) tables.Add(new RestaurantTable { Name = $"Bàn {i:D2}", Capacity = 4, Floor = 2 });
            for (int i = 18; i <= 20; i++) tables.Add(new RestaurantTable { Name = $"Bàn {i:D2}", Capacity = 6, Floor = 2 });

            _context.Tables.AddRange(tables);
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table != null)
            {
                table.Status = status;
                table.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
