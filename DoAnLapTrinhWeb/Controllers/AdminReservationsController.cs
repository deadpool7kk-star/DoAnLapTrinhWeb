using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnLapTrinhWeb.Models;

namespace DoAnLapTrinhWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminReservationsController : Controller // Quản lý đặt bàn cho admin
    {
        private readonly ApplicationDbContext _context;

        public AdminReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AdminReservations
        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations
                .OrderByDescending(r => r.BookingDate)
                .ThenByDescending(r => r.BookingTime)
                .ToListAsync();
            return View(reservations);
        }

        // POST: /AdminReservations/Confirm/5
        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Status = "Confirmed";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminReservations/Cancel/5
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Status = "Cancelled";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminReservations/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
