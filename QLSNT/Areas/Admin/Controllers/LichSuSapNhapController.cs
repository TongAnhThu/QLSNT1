using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLSNT.Areas.Admin.ViewModel;
using QLSNT.Data;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class LichSuSapNhapController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILichSuSapNhapRepository _repo;
        private readonly ITinhCuRepository _tinhCuRepo;
        private readonly ITinhMoiRepository _tinhMoiRepo;
        private readonly IXaCuRepository _xaCuRepo;
        private readonly IXaMoiRepository _xaMoiRepo;
        private readonly ISuKienHanhChinhRepository _suKienRepo;
        private readonly IHuyenCuRepository _HuyenCuRepo;
        public LichSuSapNhapController(
            ILichSuSapNhapRepository repo,
            ITinhCuRepository tinhCuRepo,
            ITinhMoiRepository tinhMoiRepo,
            IXaCuRepository xaCuRepo,
            IXaMoiRepository xaMoiRepo,
            ISuKienHanhChinhRepository suKienRepo, 
            IHuyenCuRepository huyenCuRepo, 
            ApplicationDbContext context)
        {
            _repo = repo;
            _tinhCuRepo = tinhCuRepo;
            _tinhMoiRepo = tinhMoiRepo;
            _xaCuRepo = xaCuRepo;
            _xaMoiRepo = xaMoiRepo;
            _suKienRepo = suKienRepo;
            _HuyenCuRepo = huyenCuRepo;
            _context = context;
        }

        // Index
        public async Task<IActionResult> Index(string? search)
        {
            List<LichSuSapNhap> list;

            if (string.IsNullOrWhiteSpace(search))
            {
                // lấy tất cả, include đầy đủ navigation
                list = await _repo.GetAllAsync(includeTinhs: true, includeXas: true);
            }
            else
            {
                // tìm kiếm, vẫn include navigation để view hiển thị đúng
                list = await _repo.SearchAsync(search, includeTinhs: true, includeXas: true);
            }

            ViewBag.Search = search;
            return View(list);
        }



        // Details
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Index));

            // lấy bản ghi kèm navigation
            var item = await _repo.GetByIdAsync(id, includeTinhs: true, includeXas: true);

            if (item == null)
                return RedirectToAction(nameof(Index));

            return View(item);
        }


        // Hàm nạp dropdown riêng cho tỉnh cũ/mới và xã cũ/mới
        private async Task LoadDropdowns()
        {
            var tinhCus = await _tinhCuRepo.GetAllAsync();
            ViewBag.TinhCuList = new SelectList(tinhCus ?? new List<TinhCu>(), "MaTinhCu", "TenTinhCu");

            var tinhMois = await _tinhMoiRepo.GetAllAsync();
            ViewBag.TinhMoiList = new SelectList(tinhMois ?? new List<TinhMoi>(), "MaTinhMoi", "TenTinhMoi");

            var xaCus = await _xaCuRepo.GetAllAsync();
            ViewBag.XaCuList = new SelectList(xaCus ?? new List<XaCu>(), "MaXaCu", "TenXaCu");

            var xaMois = await _xaMoiRepo.GetAllAsync();
            ViewBag.XaMoiList = new SelectList(xaMois ?? new List<XaMoi>(), "MaXaMoi", "TenXaMoi");

            // thêm sự kiện hành chính
            var suKiens = await _suKienRepo.GetAllAsync();
            ViewBag.SuKienHanhChinhList = new SelectList(
                suKiens ?? new List<SuKienHanhChinh>(),
                "SoNghiDinh", 
                "TenSK"       
            );

        }


        // GET: Create
        public async Task<IActionResult> Create()
        {
            var model = new CreateLSSN
            {
                TinhCuList = await _tinhCuRepo.GetAllAsync(),
                TinhMoiList = await _tinhMoiRepo.GetAllAsync(),
                XaCuList = await _xaCuRepo.GetAllAsync(),
                XaMoiList = await _xaMoiRepo.GetAllAsync(),
                HuyenCuList = await _HuyenCuRepo.GetAllAsync(),   // sửa đúng
                SuKienHanhChinhList = (await _suKienRepo.GetAllAsync()).ToList()
            };
            return View(model);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLSSN model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu dữ liệu không hợp lệ, nạp lại danh sách để hiển thị dropdown
                model.TinhCuList = await _tinhCuRepo.GetAllAsync();
                model.TinhMoiList = await _tinhMoiRepo.GetAllAsync();
                model.XaCuList = await _xaCuRepo.GetAllAsync();
                model.XaMoiList = await _xaMoiRepo.GetAllAsync();
                model.HuyenCuList = await _HuyenCuRepo.GetAllAsync();
                model.SuKienHanhChinhList = (await _suKienRepo.GetAllAsync()).ToList();
                                return View(model);
            }

            // Sinh mã mới cho LSSN
            var lastEntity = await _repo.GetLastAsync();
            int lastNumber = 0;
            if (lastEntity != null && !string.IsNullOrEmpty(lastEntity.MaLSSN))
            {
                var numberPart = lastEntity.MaLSSN.Substring(4);
                int.TryParse(numberPart, out lastNumber);
            }
            var newCode = "LSSN" + (lastNumber + 1).ToString("D3");

            var entity = new LichSuSapNhap
            {
                MaLSSN = newCode,
                SoNghiDinh = model.SoNghiDinh,
                NguoiTao = User.Identity?.Name,
                NgayTao = DateTime.Now,
                GhiChu = model.GhiChu,
                LssnTinhs = new List<LssnTinh>(),
                LssnXas = new List<LssnXa>(),
                Loai = model.Loai
            };

            // Nếu loại là Tỉnh
            if (model.Loai == "Tinh" && model.MaTinhCu.HasValue && model.MaTinhMoi.HasValue)
            {
                entity.LssnTinhs.Add(new LssnTinh
                {
                    MaLSSN = entity.MaLSSN,
                    MaTinhCu = model.MaTinhCu.Value,
                    MaTinhMoi = model.MaTinhMoi.Value,
                    
                });
            }

            // Nếu loại là Xã
            if (model.Loai == "Xa" && model.MaXaCu.HasValue && model.MaXaMoi.HasValue)
            {
                entity.LssnXas.Add(new LssnXa
                {
                    MaLSSN = entity.MaLSSN,
                    MaXaCu = model.MaXaCu.Value,
                    MaXaMoi = model.MaXaMoi.Value,
                    MaHuyenCu = model.MaHuyenCu.Value

                });
            }

            await _repo.AddAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        // GET: Edit
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _context.LichSuSapNhaps
                .Include(l => l.LssnTinhs)
                .Include(l => l.LssnXas)
                .FirstOrDefaultAsync(l => l.MaLSSN == id);

            if (entity == null) return NotFound();

            var model = new CreateLSSN
            {
                MaLSSN = entity.MaLSSN,
                SoNghiDinh = entity.SoNghiDinh,
                Loai = entity.LssnTinhs.Any() ? "Tinh" : entity.LssnXas.Any() ? "Xa" : null,
                MaTinhCu = entity.LssnTinhs.FirstOrDefault()?.MaTinhCu,
                MaTinhMoi = entity.LssnTinhs.FirstOrDefault()?.MaTinhMoi,
                MaXaCu = entity.LssnXas.FirstOrDefault()?.MaXaCu,
                MaXaMoi = entity.LssnXas.FirstOrDefault()?.MaXaMoi,
                GhiChu = entity.GhiChu
            };

            // 👉 Nếu có xã cũ thì join sang bảng Huyện để lấy tên huyện
            if (model.MaXaCu.HasValue)
            {
                var xaCu = await _context.XaCus
                    .Include(x => x.HuyenCu)   // join sang bảng Huyện
                    .FirstOrDefaultAsync(x => x.MaXaCu == model.MaXaCu);

                model.MaHuyenCu = xaCu?.MaHuyenCu;
                ViewBag.TenHuyenCu = xaCu?.HuyenCu?.TenHuyenCu;
            }

            // nạp danh sách cho dropdown với selected value
            var tinhsCu = await _tinhCuRepo.GetAllAsync() ?? new List<TinhCu>();
            var tinhsMoi = await _tinhMoiRepo.GetAllAsync() ?? new List<TinhMoi>();
            var xasCu = await _xaCuRepo.GetAllAsync() ?? new List<XaCu>();
            var xasMoi = await _xaMoiRepo.GetAllAsync() ?? new List<XaMoi>();
            var huyenCu = await _HuyenCuRepo.GetAllAsync() ?? new List<HuyenCu>();
            var suKiens = await _suKienRepo.GetAllAsync() ?? new List<SuKienHanhChinh>();

            ViewBag.TinhCuList = new SelectList(tinhsCu, "MaTinhCu", "TenTinhCu", model.MaTinhCu);
            ViewBag.TinhMoiList = new SelectList(tinhsMoi, "MaTinhMoi", "TenTinhMoi", model.MaTinhMoi);
            ViewBag.XaCuList = new SelectList(xasCu, "MaXaCu", "TenXaCu", model.MaXaCu);
            ViewBag.XaMoiList = new SelectList(xasMoi, "MaXaMoi", "TenXaMoi", model.MaXaMoi);
            ViewBag.HuyenCuList = new SelectList(huyenCu, "MaHuyen", "TenHuyen", model.MaHuyenCu);
            ViewBag.SuKienHanhChinhList = new SelectList(suKiens, "SoNghiDinh", "TenSK", model.SoNghiDinh);
            ViewBag.LoaiList = new SelectList(
                new[]
                {
            new { Value = "Tinh", Text = "Tỉnh" },
            new { Value = "Xa", Text = "Xã" }
                },
                "Value", "Text", model.Loai
            );

            return View(model);
        }



        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateLSSN model)
        {
            if (!ModelState.IsValid)
            {
                // nạp lại dropdown khi invalid
                model.TinhCuList = await _tinhCuRepo.GetAllAsync();
                model.TinhMoiList = await _tinhMoiRepo.GetAllAsync();
                model.XaCuList = await _xaCuRepo.GetAllAsync();
                model.XaMoiList = await _xaMoiRepo.GetAllAsync();
                model.HuyenCuList = await _HuyenCuRepo.GetAllAsync();
                model.SuKienHanhChinhList = (await _suKienRepo.GetAllAsync()).ToList();

                return View(model);
            }

            var entity = await _context.LichSuSapNhaps
                .Include(l => l.LssnTinhs)
                .Include(l => l.LssnXas)
                .FirstOrDefaultAsync(l => l.MaLSSN == model.MaLSSN);

            if (entity == null) return NotFound();

            // cập nhật thông tin cơ bản
            entity.SoNghiDinh = model.SoNghiDinh;
            entity.GhiChu = model.GhiChu;

            // xóa dữ liệu cũ
            entity.LssnTinhs.Clear();
            entity.LssnXas.Clear();

            // thêm dữ liệu mới theo loại
            if (model.Loai == "Tinh" && model.MaTinhCu.HasValue && model.MaTinhMoi.HasValue)
            {
                entity.LssnTinhs.Add(new LssnTinh
                {
                    MaLSSN = entity.MaLSSN,
                    MaTinhCu = model.MaTinhCu.Value,
                    MaTinhMoi = model.MaTinhMoi.Value
                });
            }
            else if (model.Loai == "Xa" && model.MaXaCu.HasValue && model.MaXaMoi.HasValue)
            {
                entity.LssnXas.Add(new LssnXa
                {
                    MaLSSN = entity.MaLSSN,
                    MaXaCu = model.MaXaCu.Value,
                    MaXaMoi = model.MaXaMoi.Value,
                    MaHuyenCu = model.MaHuyenCu.Value
                });
            }

            _context.Update(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }








        // Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return RedirectToAction(nameof(Index));

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
