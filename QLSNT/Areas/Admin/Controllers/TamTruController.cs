using Microsoft.AspNetCore.Mvc;
using QLSNT.Models;
using QLSNT.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLSNT.Data;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TamTruController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITamTruRepository _tamTruRepo;
        private readonly INguoiDanRepository _nguoiDanRepo;
        private readonly IXaMoiRepository _xaMoiRepo;
        private readonly ITinhMoiRepository _tinhMoiRepo;

        public TamTruController(ITamTruRepository tamTruRepo,
                                INguoiDanRepository nguoiDanRepo,
                                IXaMoiRepository xaMoiRepo,
                                ITinhMoiRepository tinhMoiRepo,
                                ApplicationDbContext context)
        {
            _tamTruRepo = tamTruRepo;
            _nguoiDanRepo = nguoiDanRepo;
            _xaMoiRepo = xaMoiRepo;
            _tinhMoiRepo = tinhMoiRepo;
            _context = context;
        }

        // GET: Admin/TamTru/Index
        public async Task<IActionResult> Index(string? keyword, int? maTinh, int? maXaMoi, DateTime? fromDate, DateTime? toDate)
        {
            // 1. Nạp danh sách Tỉnh cho dropdown lọc
            ViewBag.TinhList = await _tinhMoiRepo.GetAllAsync();

            // 2. Nạp danh sách Xã theo Tỉnh nếu đã chọn (để giữ trạng thái sau khi bấm Lọc)
            if (maTinh.HasValue)
            {
                ViewBag.XaMoiList = await _context.XaMois
                    .Where(x => x.MaTinh == maTinh.Value)
                    .OrderBy(x => x.TenXaMoi).ToListAsync();
            }
            else
            {
                ViewBag.XaMoiList = new List<XaMoi>();
            }

            // 3. Truy vấn dữ liệu từ Context để linh hoạt việc Include và Where
            var query = _context.TamTrus
                .Include(t => t.NguoiDan)
                .Include(t => t.XaMoi)
                .ThenInclude(x => x.TinhMoi)
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

            // 🔍 Lọc theo Xã (ưu tiên) hoặc Tỉnh
            if (maXaMoi.HasValue)
            {
                query = query.Where(x => x.MaXaMoi == maXaMoi.Value);
            }
            else if (maTinh.HasValue)
            {
                query = query.Where(x => x.XaMoi.MaTinh == maTinh.Value);
            }

            // 🔍 Lọc theo ngày đăng ký
            if (fromDate.HasValue) query = query.Where(x => x.NgayDangKy >= fromDate);
            if (toDate.HasValue) query = query.Where(x => x.NgayDangKy <= toDate);

            var list = await query.OrderByDescending(x => x.NgayDangKy).ToListAsync();
            return View(list);
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
        public async Task<IActionResult> Details(int maXaMoi, string maCCCD)
        {
            // Kiểm tra tham số
            if (maXaMoi == 0 || string.IsNullOrWhiteSpace(maCCCD))
                return NotFound();

            // Lấy dữ liệu từ repository
            var entity = await _tamTruRepo.GetByIdAsync(maXaMoi, maCCCD);
            if (entity == null)
                return NotFound();

            // Trả về view chi tiết
            return View(entity);
        }

        // GET: Admin/TamTru/Create
        // GET: Admin/TamTru/Create
        public async Task<IActionResult> Create(string? maCCCD)
        {
            ViewBag.NguoiDanList = new SelectList(
            await _nguoiDanRepo.GetAllAsync(),
            "MaCCCD",
            "HoTen",
            maCCCD);

ViewBag.TinhMoiList = new SelectList(
    await _tinhMoiRepo.GetAllAsync(),
    "MaTinhMoi",
    "TenTinhMoi");

            ViewBag.XaMoiList = new SelectList(
                Enumerable.Empty<XaMoi>(),
                "MaXaMoi",
                "TenXaMoi");

            var model = new TamTru
            {
                MaCCCD = maCCCD,
                NgayDangKy = DateTime.Today
            };

            return View(model);


}

        // POST: Admin/TamTru/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TamTru tamTru)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.NguoiDanList = new SelectList(
                await _nguoiDanRepo.GetAllAsync(),
                "MaCCCD",
                "HoTen",
                tamTru.MaCCCD);


    ViewBag.TinhMoiList = new SelectList(
        await _tinhMoiRepo.GetAllAsync(),
        "MaTinhMoi",
        "TenTinhMoi",
        tamTru.MaTinhMoi);

                var xaMoi = await _xaMoiRepo.GetByTinhAsync(tamTru.MaTinhMoi);

                ViewBag.XaMoiList = new SelectList(
                    xaMoi,
                    "MaXaMoi",
                    "TenXaMoi",
                    tamTru.MaXaMoi);

                return View(tamTru);
            }

            await _tamTruRepo.AddAsync(tamTru);

            var xa = await _context.XaMois
                .Include(x => x.TinhMoi)
                .FirstOrDefaultAsync(x =>
                    x.MaXaMoi == tamTru.MaXaMoi);

            var lichSu = new LichSuDiaChi
            {
                MaLichSuCuTru = Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 10),

                MaCCCD = tamTru.MaCCCD,

                MaXaMoi = tamTru.MaXaMoi,

                DiaChiCu = "",

                DiaChiMoi =
                    $"{tamTru.DiaChi}, " +
                    $"{xa?.TenXaMoi}, " +
                    $"{xa?.TinhMoi?.TenTinhMoi}",

                LoaiThayDoi = "Đăng ký tạm trú",

                NgayHieuLuc = DateTime.Now,

                NgayTao = DateTime.Now,

                NguoiTao = User.Identity?.Name
            };

            _context.LichSuDiaChis.Add(lichSu);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Thêm địa chỉ tạm trú thành công.";

            return RedirectToAction(nameof(Index));


}



        // GET: Admin/TamTru/Edit
        public async Task<IActionResult> Edit(string maCCCD, int maXaMoi)
        {
            var tamTru = await _context.TamTrus
                .Include(t => t.XaMoi)
               
                .ThenInclude(h => h.TinhMoi)
                .FirstOrDefaultAsync(t => t.MaCCCD == maCCCD && t.MaXaMoi == maXaMoi);

            if (tamTru == null) return NotFound();

            ViewBag.TenTinhMoi = tamTru.XaMoi?.TinhMoi?.TenTinhMoi;
            ViewBag.TenXaMoi = tamTru.XaMoi?.TenXaMoi;

            return View(tamTru);
        }


        // POST: Admin/TamTru/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TamTru model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.NguoiDanList = new SelectList(await _nguoiDanRepo.GetAllAsync(), "maCCCD", "HoTen", model.MaCCCD);
                ViewBag.XaMoiList = new SelectList(await _xaMoiRepo.GetAllAsync(), "MaXaMoi", "TenXaMoi", model.MaXaMoi);
                return View(model);
            }

            await _tamTruRepo.UpdateAsync(model);
            TempData["SuccessMessage"] = "Cập nhật địa chỉ tạm trú thành công.";
            return RedirectToAction(nameof(Index), new { maCCCD = model.MaCCCD });
        }

        // GET: Admin/TamTru/Delete
        public async Task<IActionResult> Delete(int maXaMoi, string maCCCD)
        {
            var entity = await _tamTruRepo.GetByIdAsync(maXaMoi, maCCCD);
            if (entity == null) return NotFound();

            return View(entity);
        }

        // POST: Admin/TamTru/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int maXaMoi, string maCCCD)
        {
            await _tamTruRepo.DeleteAsync(maXaMoi, maCCCD);
            TempData["SuccessMessage"] = "Xoá địa chỉ tạm trú thành công.";
            return RedirectToAction(nameof(Index), new { maCCCD });
        }
        [HttpGet]
        public IActionResult GetNguoiDan(string cccd)
        {
            var nguoiDan = _context.NguoiDans
                .FirstOrDefault(x => x.MaCCCD == cccd);

            if (nguoiDan == null)
                return Json(null);

            return Json(new
            {
                hoTen = nguoiDan.HoTen,
                ngaySinh = nguoiDan.NgaySinh?.ToString("dd/MM/yyyy"),
                gioiTinh = nguoiDan.GioiTinh
            });
        }
    }
}
