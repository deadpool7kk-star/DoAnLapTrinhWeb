using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DoAnLapTrinhWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DoAnLapTrinhWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dishes = await _context.Dishes
                .Include(d => d.Category)
                .Where(d => d.IsVisible && d.Category.Name != "Trạng Miệng")
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.Name != "Trạng Miệng")
                .ToListAsync();

            ViewBag.Categories = categories;
            return View(dishes);
        }

        [HttpPost]
        public async Task<IActionResult> BookTable(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.CreatedAt = DateTime.Now;
                reservation.Status = "Pending";
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                
                TempData["Message"] = "Đặt bàn thành công! Chúng tôi sẽ sớm liên hệ xác nhận qua điện thoại.";
                return RedirectToAction(nameof(Index), "Home", new { area = "" }, "reservation");
            }
            
            TempData["Error"] = "Vui lòng kiểm tra lại thông tin đăng ký.";
            return RedirectToAction(nameof(Index), "Home", new { area = "" }, "reservation");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
