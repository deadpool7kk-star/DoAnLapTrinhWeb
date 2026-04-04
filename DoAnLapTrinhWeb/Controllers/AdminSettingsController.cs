using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnLapTrinhWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DoAnLapTrinhWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AdminSettings
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfYear = new DateTime(now.Year, 1, 1);

            // Thống kê doanh thu (Báo cáo Mục 3)
            ViewBag.RevenueMonth = await _context.Invoices
                .Where(i => i.CreatedAt >= startOfMonth && i.Status == "Paid")
                .SumAsync(i => i.TotalAmount);

            ViewBag.RevenueYear = await _context.Invoices
                .Where(i => i.CreatedAt >= startOfYear && i.Status == "Paid")
                .SumAsync(i => i.TotalAmount);

            ViewBag.ActiveInvoicesCount = await _context.Invoices
                .CountAsync(i => !i.IsArchived);

            ViewBag.TotalInvoicesMonth = await _context.Invoices
                .CountAsync(i => i.CreatedAt >= startOfMonth);

            return View();
        }

        // POST: /AdminSettings/ResetMonthlyData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetMonthlyData()
        {
            try
            {
                // Tìm tất cả hóa đơn chưa lưu trữ
                var activeInvoices = await _context.Invoices
                    .Where(i => !i.IsArchived)
                    .ToListAsync();

                foreach (var inv in activeInvoices)
                {
                    inv.IsArchived = true;
                }

                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Đã kết toán thành công. Danh sách hóa đơn hiện tại đã được làm sạch." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi kết toán: " + ex.Message });
            }
        }

        // POST: /AdminSettings/ClearAllData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearAllData()
        {
            try
            {
                // 1. Xóa chi tiết hóa đơn (Phải xóa trước do FK)
                var items = await _context.InvoiceItems.ToListAsync();
                _context.InvoiceItems.RemoveRange(items);

                // 2. Xóa hóa đơn
                var invoices = await _context.Invoices.ToListAsync();
                _context.Invoices.RemoveRange(invoices);

                // 3. Xóa đơn đặt bàn
                var reservations = await _context.Reservations.ToListAsync();
                _context.Reservations.RemoveRange(reservations);

                // 4. Giải phóng toàn bộ bàn về "Available"
                var tables = await _context.Tables.ToListAsync();
                foreach (var t in tables)
                {
                    t.Status = "Available";
                }

                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Toàn bộ dữ liệu test đã được xóa sạch. Hệ thống đã sẵn sàng cho vận hành thực tế." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa dữ liệu: " + ex.Message });
            }
        }
    }
}
