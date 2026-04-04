using Microsoft.AspNetCore.Mvc;
using DoAnLapTrinhWeb.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoAnLapTrinhWeb.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBooking(Reservation model)
        {
            if (ModelState.IsValid)
            {
                // Nếu User đang đăng nhập, lưu UserId
                if (User.Identity.IsAuthenticated)
                {
                    model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                _context.Reservations.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đặt bàn thành công! Chúng tôi sẽ liên hệ lại với bạn sớm nhất.";
                return RedirectToAction("Index", "Reservation"); 
            }
            
            TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin đặt bàn hợp lệ.";
            return RedirectToAction("Index", "Reservation");
        }
    }
}
