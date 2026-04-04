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
                .Include(r => r.Table)
                .OrderByDescending(r => r.BookingDate)
                .ThenByDescending(r => r.BookingTime)
                .ToListAsync();
            
            ViewBag.AllTables = await _context.Tables
                .OrderBy(t => t.Floor)
                .ThenBy(t => t.Name)
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

        // POST: /AdminReservations/AssignTable
        [HttpPost]
        public async Task<IActionResult> AssignTable(int id, int tableId)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            var table = await _context.Tables.FindAsync(tableId);

            if (reservation != null && table != null)
            {
                // Kiểm tra xem bàn có thực sự còn trống không (để tránh race condition)
                if (table.Status != "Available")
                {
                    return Json(new { success = false, message = $"Bàn {table.Name} hiện đã có khách hoặc đã được đặt trước. Vui lòng chọn bàn khác." });
                }

                reservation.TableId = tableId;
                reservation.Status = "Seated";
                table.Status = "Occupied";
                
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Không tìm thấy thông tin yêu cầu hoặc bàn." });
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
            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation != null)
            {
                // 1. Giải quyết xung đột khóa ngoại: Tìm các hóa đơn liên quan và gỡ bỏ liên kết ReservationId
                var relatedInvoices = await _context.Invoices.Where(i => i.ReservationId == id).ToListAsync();
                foreach (var inv in relatedInvoices)
                {
                    inv.ReservationId = null; // Gỡ bỏ liên kết để có thể xóa Reservation mà không lỗi FK
                }

                // 2. Phải SaveChanges cho hóa đơn trước hoặc đồng thời với việc xóa Reservation
                // Tuy nhiên trong EF, nếu ta dùng cùng một context, nó sẽ tự xử lý thứ tự.

                // 3. Giải phóng bàn nếu đang ở trạng thái gắn bàn
                if (reservation.TableId.HasValue && reservation.Table != null)
                {
                    reservation.Table.Status = "Available";
                }

                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminReservations/GetDetails/5
        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null) return NotFound();

            return Json(new {
                fullName = reservation.FullName,
                phone = reservation.Phone,
                date = reservation.BookingDate.ToString("dd/MM/yyyy"),
                time = reservation.BookingTime,
                guests = reservation.GuestCount,
                note = reservation.Note ?? "—",
                status = reservation.Status,
                createdAt = reservation.CreatedAt.ToString("HH:mm dd/MM/yyyy"),
                tableName = reservation.Table?.Name ?? "Chưa xếp"
            });
        }
    }
}
