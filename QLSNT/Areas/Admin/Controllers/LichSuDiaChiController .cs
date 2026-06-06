using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class LichSuDiaChiController : Controller
    {
        private readonly ILichSuDiaChiRepository _lsctRepo;
        private readonly INguoiDanRepository _nguoiDanRepo;
        private readonly IXaMoiRepository _xaMoiRepo; // Chỉ còn xã mới

        public LichSuDiaChiController(
            ILichSuDiaChiRepository lsctRepo,
            INguoiDanRepository nguoiDanRepo,
            IXaMoiRepository xaMoiRepo)
        {
            _lsctRepo = lsctRepo;
            _nguoiDanRepo = nguoiDanRepo;
            _xaMoiRepo = xaMoiRepo;
        }

        // GET: Admin/LichSuDiaChi
        public async Task<IActionResult> Index(string? searchCccd)
        {
            var list = await _lsctRepo.GetAllAsync(searchCccd);
            ViewBag.SearchCccd = searchCccd;
            return View(list);
        }

        // GET: Admin/LichSuDiaChi/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var item = await _lsctRepo.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }
        private async Task<string> GenerateMaLichSuCuTruAsync()
        {
            var lastCode = await _lsctRepo.GetLastCodeAsync();
            int number = 0;

            if (!string.IsNullOrEmpty(lastCode) && lastCode.Length > 2)
            {
                // bỏ tiền tố TT và parse số
                int.TryParse(lastCode.Substring(2), out number);
            }

            // tăng số lên 1 và format lại
            return $"TT{(number + 1):D3}";
        }

        // GET: Admin/LichSuDiaChi/Create
        public async Task<IActionResult> Create(string? maCccd)
        {
            await LoadDropdowns(maCccd: maCccd);

            var model = new LichSuDiaChi
            {
                MaCCCD = maCccd,
                NgayHieuLuc = DateTime.Today
            };

            return View(model);
        }

        // POST: Admin/LichSuDiaChi/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LichSuDiaChi model)
        {
            ModelState.Remove(nameof(model.MaLichSuCuTru));
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model.MaCCCD, model.MaXaMoi);
                return View(model);
            }

            // Sinh mã tự động TT001, TT002...
            model.MaLichSuCuTru = await GenerateMaLichSuCuTruAsync();
            model.NguoiTao = User.Identity?.Name;
            model.NgayTao = DateTime.Now;

            await _lsctRepo.AddAsync(model);

            TempData["SuccessMessage"] = "Thêm lịch sử cư trú thành công.";
            return RedirectToAction(nameof(Index), new { searchCccd = model.MaCCCD });
        }


        // GET: Admin/LichSuDiaChi/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var entity = await _lsctRepo.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            await LoadDropdowns(entity.MaCCCD, entity.MaXaMoi);
            return View(entity);
        }

        // POST: Admin/LichSuDiaChi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, LichSuDiaChi model)
        {
            if (id != model.MaLichSuCuTru)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model.MaCCCD, model.MaXaMoi);
                return View(model);
            }

            var entity = await _lsctRepo.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            entity.LoaiThayDoi = model.LoaiThayDoi;
            entity.SoQuyetDinh = model.SoQuyetDinh;
            entity.LyDoThayDoi = model.LyDoThayDoi;
            entity.NgayHieuLuc = model.NgayHieuLuc;
            entity.NgayKetThuc = model.NgayKetThuc;
            entity.DiaChiCu = model.DiaChiCu;
            entity.DiaChiMoi = model.DiaChiMoi;
            entity.GhiChu = model.GhiChu;
            entity.MaCCCD = model.MaCCCD;
            entity.MaXaMoi = model.MaXaMoi;

            entity.NguoiCapNhat = User.Identity?.Name;
            entity.NgayCapNhat = DateTime.Now;

            await _lsctRepo.UpdateAsync(entity);

            TempData["SuccessMessage"] = "Cập nhật lịch sử cư trú thành công.";
            return RedirectToAction(nameof(Index), new { searchCccd = model.MaCCCD });
        }

        // GET: Admin/LichSuDiaChi/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var entity = await _lsctRepo.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            return View(entity);
        }

        // POST: Admin/LichSuDiaChi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var entity = await _lsctRepo.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            await _lsctRepo.DeleteAsync(id);

            TempData["SuccessMessage"] = "Xoá lịch sử cư trú thành công.";
            return RedirectToAction(nameof(Index), new { searchCccd = entity.MaCCCD });
        }

        /// <summary>
        /// Load dropdown cho NguoiDan và XaMoi
        /// </summary>
        private async Task LoadDropdowns(string? maCccd = null, int? maXaMoi = null)
        {
            var nguoiDans = await _nguoiDanRepo.GetAllAsync();
            var xasMoi = await _xaMoiRepo.GetAllAsync();

            ViewBag.NguoiDanList = new SelectList(nguoiDans, "MaCCCD", "HoTen", maCccd);
            ViewBag.XaMoiList = new SelectList(xasMoi, "MaXaMoi", "TenXaMoi", maXaMoi);
        }
    }
}
