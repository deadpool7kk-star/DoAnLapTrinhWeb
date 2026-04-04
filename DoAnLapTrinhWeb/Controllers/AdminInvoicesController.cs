using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnLapTrinhWeb.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DoAnLapTrinhWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminInvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminInvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminInvoices
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Invoices.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(i => i.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(i => i.CreatedAt <= endDate.Value);

            var invoices = await query.OrderByDescending(i => i.CreatedAt).ToListAsync();

            // Calculate revenue (only paid invoices)
            ViewBag.TotalRevenue = query.Where(i => i.Status == "Paid").Sum(i => i.TotalAmount);
            ViewBag.InvoiceCount = invoices.Count;
            ViewBag.PaidCount = invoices.Count(i => i.Status == "Paid");

            return View(invoices);
        }

        // GET: AdminInvoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _context.Invoices
                .Include(i => i.Table)
                .Include(i => i.InvoiceItems)
                .ThenInclude(it => it.Dish)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (invoice == null) return NotFound();

            return View(invoice);
        }

        // GET: /AdminInvoices/Create
        public IActionResult Create(int? tableId, string? customerName, int? reservationId)
        {
            ViewBag.Tables = _context.Tables.ToList();
            ViewBag.Dishes = _context.Dishes.ToList();
            ViewBag.CustomerName = customerName ?? "";
            ViewBag.SelectedTableId = tableId;
            ViewBag.ReservationId = reservationId;
            return View();
        }

        // POST: AdminInvoices/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try {
                    var invoice = new Invoice
                    {
                        CustomerName = model.CustomerName,
                        TableId = model.TableId,
                        ReservationId = model.ReservationId,
                        Status = "Paid",
                        PaymentMethod = model.PaymentMethod ?? "Cash",
                        CreatedAt = DateTime.Now,
                        TotalAmount = 0
                    };

                    _context.Invoices.Add(invoice);
                    await _context.SaveChangesAsync(); // Lưu trước để có ID

                    decimal total = 0;
                    foreach (var item in model.Items)
                    {
                        var dish = await _context.Dishes.FindAsync(item.DishId);
                        if (dish != null)
                        {
                            var invoiceItem = new InvoiceItem
                            {
                                InvoiceId = invoice.Id, // Bây giờ đã có ID thật
                                DishId = dish.Id,
                                DishName = dish.Name,
                                OriginalPrice = dish.Price,
                                DiscountPercentage = dish.DiscountPercentage.GetValueOrDefault(),
                                UnitPrice = dish.Price * (1 - (decimal)dish.DiscountPercentage.GetValueOrDefault() / 100),
                                Quantity = item.Quantity
                            };
                            _context.InvoiceItems.Add(invoiceItem);
                            total += (invoiceItem.UnitPrice * item.Quantity);
                        }
                    }

                    invoice.TotalAmount = total;

                    // Cập nhật trạng thái bàn nếu có
                    if (model.TableId.HasValue)
                    {
                        var table = await _context.Tables.FindAsync(model.TableId.Value);
                        if (table != null)
                        {
                            table.Status = "Available";
                        }
                    }

                    // Cập nhật trạng thái đơn đặt bàn nếu có
                    if (model.ReservationId.HasValue)
                    {
                        var reservation = await _context.Reservations.FindAsync(model.ReservationId.Value);
                        if (reservation != null)
                        {
                            reservation.Status = "Completed";
                        }
                    }

                    await _context.SaveChangesAsync();
                    return Json(new { success = true, invoiceId = invoice.Id });
                }
                catch (Exception ex) {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return BadRequest(ModelState);
        }

        // Mark as Paid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                invoice.Status = "Paid";
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return NotFound();
        }
    }

    public class InvoiceCreateViewModel
    {
        public string CustomerName { get; set; } = "";
        public int? TableId { get; set; }
        public int? ReservationId { get; set; }
        public string? PaymentMethod { get; set; }
        public List<InvoiceItemViewModel> Items { get; set; } = new List<InvoiceItemViewModel>();
    }

    public class InvoiceItemViewModel
    {
        public int DishId { get; set; }
        public int Quantity { get; set; }
    }
}

