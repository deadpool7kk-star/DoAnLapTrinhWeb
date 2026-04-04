using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnLapTrinhWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnLapTrinhWeb.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NotificationBellViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var pendingReservations = await _context.Reservations
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            var totalPending = await _context.Reservations
                .CountAsync(r => r.Status == "Pending");

            ViewBag.TotalPending = totalPending;

            return View(pendingReservations);
        }
    }
}
