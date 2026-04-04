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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminMenuController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /AdminMenu
        public async Task<IActionResult> Index()
        {
            var dishes = await _context.Dishes
                .Include(d => d.Category)
                .Where(d => d.Category.Name != "Trạng Miệng")
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.Name != "Trạng Miệng")
                .ToListAsync();
            
            ViewBag.Categories = categories;
            
            return View(dishes);
        }

        // POST: /AdminMenu/Create
        [HttpPost]
        public async Task<IActionResult> Create(Dish dish, IFormFile? imageFile)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                // Xử lý Upload Ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    dish.ImageUrl = await SaveImage(imageFile);
                }

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
        public async Task<IActionResult> Edit(Dish dish, IFormFile? imageFile)
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

                        // Xử lý Upload Ảnh Mới
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            // Xoá ảnh cũ nếu có
                            DeleteImageFile(existingDish.ImageUrl);
                            // Lưu ảnh mới
                            dish.ImageUrl = await SaveImage(imageFile);
                        }
                        else
                        {
                            // Giữ lại ảnh cũ nếu không upload ảnh mới
                            dish.ImageUrl = existingDish.ImageUrl;
                        }

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
                // Xoá file ảnh vật lý
                DeleteImageFile(dish.ImageUrl);

                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // --- HELPER METHODS ---
        private async Task<string> SaveImage(IFormFile file)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "dishes");
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/images/dishes/" + uniqueFileName;
        }

        private void DeleteImageFile(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.Id == id);
        }
    }
}
