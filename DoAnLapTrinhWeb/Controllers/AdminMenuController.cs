using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnLapTrinhWeb.Models;

namespace DoAnLapTrinhWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminMenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminMenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AdminMenu
        public async Task<IActionResult> Index()
        {
            var dishes = await _context.Dishes.Include(d => d.Category).ToListAsync();
            var categories = await _context.Categories.ToListAsync();
            
            ViewBag.Categories = categories;
            
            return View(dishes);
        }

        // POST: /AdminMenu/Create
        [HttpPost]
        public async Task<IActionResult> Create(Dish dish)
        {
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                dish.CreatedAt = DateTime.Now;
                dish.UpdatedAt = DateTime.Now;
                // Xử lý chống lỗi NULL cho các cột mới
                dish.Description ??= "";
                dish.Badge ??= "";
                dish.Icon ??= "";
                
                _context.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminMenu/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(Dish dish)
        {
            // Loại bỏ hoàn toàn validation cho các trường điều hướng và trường không bắt buộc
            ModelState.Remove("Category");
            
            if (ModelState.IsValid)
            {
                try
                {
                    var existingDish = await _context.Dishes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dish.Id);
                    if (existingDish != null)
                    {
                        // Giữ nguyên ngày tạo ban đầu
                        dish.CreatedAt = existingDish.CreatedAt;
                        // Cập nhật ngày sửa mới nhất
                        dish.UpdatedAt = DateTime.Now;

                        // Xử lý chống lỗi NULL cho các cột bắt buộc trong Database
                        dish.Description ??= "";
                        dish.Badge ??= "";
                        dish.Icon ??= "";
                        
                        _context.Update(dish);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.Id)) return NotFound();
                    else throw;
                }
            }
            
            // Nếu có lỗi, reload trang Index
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminMenu/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.Id == id);
        }
    }
}
