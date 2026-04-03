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
            if (ModelState.IsValid)
            {
                dish.CreatedAt = DateTime.Now;
                dish.UpdatedAt = DateTime.Now;
                _context.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminMenu/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Dish dish)
        {
            if (id != dish.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDish = await _context.Dishes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
                    if (existingDish != null)
                    {
                        dish.CreatedAt = existingDish.CreatedAt;
                        dish.UpdatedAt = DateTime.Now;
                        _context.Update(dish);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
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
