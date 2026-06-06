using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class LssnTinhController : Controller
    {
        private readonly ILssnTinhRepository _lssnTinhRepo;
        private readonly ILichSuSapNhapRepository _lssnRepo;
        private readonly ITinhCuRepository _tinhCuRepo;
        private readonly ITinhMoiRepository _tinhMoiRepo;

        public LssnTinhController(
            ILssnTinhRepository lssnTinhRepo,
            ILichSuSapNhapRepository lssnRepo,
            ITinhCuRepository tinhCuRepo,
            ITinhMoiRepository tinhMoiRepo)
        {
            _lssnTinhRepo = lssnTinhRepo;
            _lssnRepo = lssnRepo;
            _tinhCuRepo = tinhCuRepo;
            _tinhMoiRepo = tinhMoiRepo;
        }

        // ================== INDEX ==================
        // GET: /Admin/LssnTinh
        public async Task<IActionResult> Index()
        {
            var list = await _lssnTinhRepo.GetAllAsync();
            return View(list);
        }

        // ================== DETAILS ==================
        // GET: /Admin/LssnTinh/Details/LS001
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var item = await _lssnTinhRepo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // ================== CREATE ==================
        // GET: /Admin/LssnTinh/Create?maLssn=LS001
        [HttpGet]
        public async Task<IActionResult> Create(string maLssn)
        {
            if (string.IsNullOrWhiteSpace(maLssn)) return NotFound();

            await LoadTinhDropDownsAsync();

            var model = new LssnTinh
            {
                MaLSSN = maLssn   // MaLSSN là string
            };

            var lssn = await _lssnRepo.GetByIdAsync(model.MaLSSN);
            ViewBag.LichSuSapNhap = lssn;

            return View(model);
        }

        // POST: /Admin/LssnTinh/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LssnTinh model)
        {
            if (!ModelState.IsValid)
            {
                await LoadTinhDropDownsAsync(model.MaTinhCu, model.MaTinhMoi);
                var lssn = await _lssnRepo.GetByIdAsync(model.MaLSSN);
                ViewBag.LichSuSapNhap = lssn;
                return View(model);
            }

            await _lssnTinhRepo.AddAsync(model);
            // Hợp lý hơn là quay về chi tiết lần sáp nhập
            return RedirectToAction("Details", "LichSuSapNhap", new { id = model.MaLSSN });
        }

        // ================== EDIT ==================
        // GET: /Admin/LssnTinh/Edit/LS001
        // GET: /Admin/LssnTinh/Edit/LS001
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var item = await _lssnTinhRepo.GetByIdAsync(id);
            if (item == null) return NotFound();

            // load dropdown tỉnh, truyền giá trị đang chọn
            await LoadTinhDropDownsAsync(item.MaTinhCu, item.MaTinhMoi);

            // lấy thông tin lịch sử sáp nhập cha để hiển thị thêm
            ViewBag.LichSuSapNhap = await _lssnRepo.GetByIdAsync(item.MaLSSN);

            return View(item);
        }

        // POST: /Admin/LssnTinh/Edit/LS001
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, LssnTinh model)
        {
            if (id != model.MaLSSN) return BadRequest();

            if (!ModelState.IsValid)
            {
                // reload dropdown khi có lỗi
                await LoadTinhDropDownsAsync(model.MaTinhCu, model.MaTinhMoi);
                ViewBag.LichSuSapNhap = await _lssnRepo.GetByIdAsync(model.MaLSSN);
                return View(model);
            }

            var entity = await _lssnTinhRepo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            // cập nhật các trường cần thiết
            entity.MaTinhCu = model.MaTinhCu;
            entity.MaTinhMoi = model.MaTinhMoi;
            entity.MaLSSN = model.MaLSSN;

            await _lssnTinhRepo.UpdateAsync(entity);

            return RedirectToAction("Details", "LichSuSapNhap", new { id = model.MaLSSN });
        }


        // ================== DELETE ==================
        // GET: /Admin/LssnTinh/Delete/LS001
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var item = await _lssnTinhRepo.GetByIdAsync(id);
            if (item == null) return NotFound();

            var lssn = await _lssnRepo.GetByIdAsync(item.MaLSSN);
            ViewBag.LichSuSapNhap = lssn;

            return View(item);
        }

        // POST: /Admin/LssnTinh/Delete/LS001
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var item = await _lssnTinhRepo.GetByIdAsync(id);
            if (item == null) return NotFound();

            var maLssn = item.MaLSSN;

            await _lssnTinhRepo.DeleteAsync(id);
            return RedirectToAction("Details", "LichSuSapNhap", new { id = maLssn });
        }

        // ================== HELPER: LOAD DROPDOWN TỈNH ==================
        private async Task LoadTinhDropDownsAsync(int? selectedTinhCu = null, int? selectedTinhMoi = null)
        {
            var tinhCuList = await _tinhCuRepo.GetAllAsync();
            var tinhMoiList = await _tinhMoiRepo.GetAllAsync();

            ViewBag.TinhCuList = new SelectList(tinhCuList, "MaTinhCu", "TenTinhCu", selectedTinhCu);
            ViewBag.TinhMoiList = new SelectList(tinhMoiList, "MaTinhMoi", "TenTinhMoi", selectedTinhMoi);
            // chú ý: TenTinhMoi (không phải TenTinh)
        }
    }
}
