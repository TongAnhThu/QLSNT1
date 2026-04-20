using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLSNT.Data;
using QLSNT.Models;

namespace QLSNT.Controllers
{
    [Area("Admin")]
    public class ThuongTruController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThuongTruController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ThuongTru
        public async Task<IActionResult> Index(string keyword, int? maTinh, int? maXaMoi, DateTime? fromDate, DateTime? toDate)
        {
            // 1. Nạp danh sách Tỉnh cho dropdown (Khắc phục lỗi NullReferenceException)
            ViewBag.TinhList = await _context.TinhMois.OrderBy(t => t.TenTinhMoi).ToListAsync();

            // 2. Nạp danh sách Xã dựa trên Tỉnh đã chọn (để giữ trạng thái dropdown Xã sau khi Lọc)
            if (maTinh.HasValue)
            {
                ViewBag.XaMoiList = await _context.XaMois
                    .Where(x => x.MaTinh == maTinh.Value)
                    .OrderBy(x => x.TenXaMoi).ToListAsync();
            }
            else
            {
                // Nếu chưa chọn tỉnh, XaMoiList sẽ rỗng hoặc chứa toàn bộ tùy theo nhu cầu
                ViewBag.XaMoiList = new List<XaMoi>();
            }

            var query = _context.ThuongTrus
                .Include(x => x.NguoiDan)
                .Include(x => x.XaMoi)
                .ThenInclude(x => x.TinhMoi) // Include thêm Tỉnh nếu cần hiển thị tên tỉnh trên bảng
                .AsQueryable();

            // 🔍 Lọc theo từ khóa (CCCD, Họ tên, Địa chỉ)
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(x =>
                    x.MaCCCD.Contains(keyword) ||
                    x.NguoiDan.HoTen.ToLower().Contains(keyword) ||
                    x.DiaChi.ToLower().Contains(keyword)
                );
            }

            // 🔍 Lọc theo Tỉnh (Nếu có chọn tỉnh nhưng chưa chọn xã cụ thể)
            if (maTinh.HasValue && !maXaMoi.HasValue)
            {
                query = query.Where(x => x.XaMoi.MaTinh == maTinh.Value);
            }

            // 🔍 Lọc theo Xã
            if (maXaMoi.HasValue)
            {
                query = query.Where(x => x.MaXaMoi == maXaMoi.Value);
            }

            // 🔍 Lọc theo ngày
            if (fromDate.HasValue)
            {
                query = query.Where(x => x.NgayDangKy >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.NgayDangKy <= toDate);
            }

            var result = await query.OrderByDescending(x => x.NgayDangKy).ToListAsync();
            return View(result);
        }

        // GET: ThuongTru/Details/5
        public async Task<IActionResult> Details(int? maXaMoi, string? maCCCD)
        {
            if (maXaMoi == null || maCCCD == null)
                return NotFound();

            var thuongTru = await _context.ThuongTrus
                .Include(t => t.XaMoi)
                .Include(t => t.NguoiDan)
                .FirstOrDefaultAsync(m => m.MaXaMoi == maXaMoi && m.MaCCCD == maCCCD);

            if (thuongTru == null)
                return NotFound();

            return View(thuongTru);
        }

        // GET: ThuongTru/Create
        public IActionResult Create()
        {
            ViewData["MaXaMoi"] = new SelectList(_context.XaMois.OrderBy(x => x.TenXaMoi), "MaXaMoi", "TenXaMoi");
            ViewData["MaCCCD"] = new SelectList(_context.NguoiDans.OrderBy(n => n.HoTen), "MaCCCD", "HoTen");
            return View();
        }

        // POST: ThuongTru/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaXaMoi,MaCCCD,DiaChi,NgayDangKy")] ThuongTru thuongTru)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng khóa chính trước khi thêm
                if (ThuongTruExists(thuongTru.MaXaMoi, thuongTru.MaCCCD))
                {
                    ModelState.AddModelError("", "Dữ liệu thường trú cho người dân này tại xã đã tồn tại.");
                }
                else
                {
                    _context.Add(thuongTru);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["MaXaMoi"] = new SelectList(_context.XaMois, "MaXaMoi", "TenXaMoi", thuongTru.MaXaMoi);
            ViewData["MaCCCD"] = new SelectList(_context.NguoiDans, "MaCCCD", "HoTen", thuongTru.MaCCCD);
            return View(thuongTru);
        }

        // GET: ThuongTru/Edit
        public async Task<IActionResult> Edit(int? maXaMoi, string? maCCCD)
        {
            if (maXaMoi == null || maCCCD == null)
                return NotFound();

            var thuongTru = await _context.ThuongTrus
                .Include(t => t.NguoiDan)
                .Include(t => t.XaMoi)
                .FirstOrDefaultAsync(x => x.MaXaMoi == maXaMoi && x.MaCCCD == maCCCD);

            if (thuongTru == null)
                return NotFound();

            return View(thuongTru);
        }

        // POST: ThuongTru/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int maXaMoi, string maCCCD, [Bind("MaXaMoi,MaCCCD,DiaChi,NgayDangKy")] ThuongTru thuongTru)
        {
            if (maXaMoi != thuongTru.MaXaMoi || maCCCD != thuongTru.MaCCCD)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thuongTru);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThuongTruExists(maXaMoi, maCCCD))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(thuongTru);
        }

        // GET: ThuongTru/Delete
        public async Task<IActionResult> Delete(int? maXaMoi, string? maCCCD)
        {
            if (maXaMoi == null || maCCCD == null)
                return NotFound();

            var thuongTru = await _context.ThuongTrus
                .Include(t => t.XaMoi)
                .Include(t => t.NguoiDan)
                .FirstOrDefaultAsync(m => m.MaXaMoi == maXaMoi && m.MaCCCD == maCCCD);

            if (thuongTru == null)
                return NotFound();

            return View(thuongTru);
        }

        // POST: ThuongTru/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int maXaMoi, string maCCCD)
        {
            var thuongTru = await _context.ThuongTrus.FindAsync(maXaMoi, maCCCD);
            if (thuongTru != null)
            {
                _context.ThuongTrus.Remove(thuongTru);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // API trả về JSON cho Ajax gọi từ View
        [HttpGet]
        public async Task<JsonResult> GetXaByTinh(int maTinh)
        {
            var xas = await _context.XaMois
                      .Where(x => x.MaTinh == maTinh)
                      .OrderBy(x => x.TenXaMoi)
                      .Select(x => new { maXaMoi = x.MaXaMoi, tenXaMoi = x.TenXaMoi })
                      .ToListAsync();
            return Json(xas);
        }

        private bool ThuongTruExists(int maXaMoi, string maCCCD)
        {
            return _context.ThuongTrus.Any(e => e.MaXaMoi == maXaMoi && e.MaCCCD == maCCCD);
        }
    }
}