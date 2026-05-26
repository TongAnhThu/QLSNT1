using Microsoft.AspNetCore.Mvc;
using QLSNT.Data;
using QLSNT.Models;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard/Home page
        public IActionResult Index()
        {
            // Example: Get counts for dashboard
            var nguoiDanCount = _context.NguoiDans.Count();
            var tamTruCount = _context.TamTrus.Count();
            var thuongTruCount = _context.ThuongTrus.Count();

            ViewBag.NguoiDanCount = nguoiDanCount;
            ViewBag.TamTruCount = tamTruCount;
            ViewBag.ThuongTruCount = thuongTruCount;

            return View();
        }

        // About page
        public IActionResult About()
        {
            ViewBag.Message = "Trang quản trị hệ thống quản lý sổ nhân tạm.";
            return View();
        }

        // Error page
        public IActionResult Error()
        {
            return View();
        }
    }
}
