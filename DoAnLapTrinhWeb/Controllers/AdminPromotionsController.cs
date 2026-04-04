using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnLapTrinhWeb.Models;

namespace DoAnLapTrinhWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPromotionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminPromotionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AdminPromotions
        public async Task<IActionResult> Index()
        {
            var dishes = await _context.Dishes.Include(d => d.Category).ToListAsync();
            return View(dishes);
        }

        // POST: /AdminPromotions/UpdateDiscount
        [HttpPost]
        public async Task<IActionResult> UpdateDiscount(int id, int? discountPercentage)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                dish.DiscountPercentage = discountPercentage;
                dish.UpdatedAt = DateTime.Now;
                
                // Tự động gắn nhãn SALE nếu có giảm giá
                if (discountPercentage.HasValue && discountPercentage.Value > 0)
                {
                    dish.Badge = "SALE -" + discountPercentage.Value + "%";
                }
                else
                {
                    // Nếu xoá khuyến mãi, xoá nhãn SALE
                    if (dish.Badge != null && dish.Badge.StartsWith("SALE")) dish.Badge = null;
                }

                _context.Update(dish);
                await _context.SaveChangesAsync();
                return Json(new { success = true, discountedPrice = dish.DiscountedPrice.ToString("N0") + "₫" });
            }
            return Json(new { success = false });
        }
    }
}
