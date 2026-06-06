using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class LssnXaController : Controller
    {
        private readonly ILssnXaRepository _lssnXaRepo;
        private readonly ILichSuSapNhapRepository _lssnRepo;
        private readonly IXaCuRepository _xaCuRepo;
        private readonly IXaMoiRepository _xaMoiRepo;

        public LssnXaController(
            ILssnXaRepository lssnXaRepo,
            ILichSuSapNhapRepository lssnRepo,
            IXaCuRepository xaCuRepo,
            IXaMoiRepository xaMoiRepo)
        {
            _lssnXaRepo = lssnXaRepo;
            _lssnRepo = lssnRepo;
            _xaCuRepo = xaCuRepo;
            _xaMoiRepo = xaMoiRepo;
        }

        // ==================== LIST THEO 1 LẦN SÁP NHẬP ====================
        // GET: /Admin/LssnXa/ByLssn?maLssn=LS001
        public async Task<IActionResult> ByLssn(string maLssn)
        {
            if (string.IsNullOrWhiteSpace(maLssn)) return NotFound();

            var lssn = await _lssnRepo.GetByIdAsync(maLssn);
            if (lssn == null) return NotFound();

            var list = await _lssnXaRepo.GetByLssnAsync(maLssn);

            ViewBag.LichSuSapNhap = lssn;
            return View(list);
        }

        // ==================== CREATE ====================
        // GET: /Admin/LssnXa/Create?maLssn=LS001
        [HttpGet]
        public async Task<IActionResult> Create(string maLssn)
        {
            if (string.IsNullOrWhiteSpace(maLssn)) return NotFound();

            var lssn = await _lssnRepo.GetByIdAsync(maLssn);
            if (lssn == null) return NotFound();

            await LoadXaDropDownsAsync();

            var model = new LssnXa
            {
                MaLSSN = maLssn
            };

            ViewBag.LichSuSapNhap = lssn;
            return View(model);
        }

        // POST: /Admin/LssnXa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LssnXa model)
        {
            if (!ModelState.IsValid)
            {
                await LoadXaDropDownsAsync(model.MaXaCu, model.MaXaMoi);
                var lssn = await _lssnRepo.GetByIdAsync(model.MaLSSN);
                ViewBag.LichSuSapNhap = lssn;

                return View(model);
            }

            await _lssnXaRepo.AddAsync(model);
            return RedirectToAction("Details", "LichSuSapNhap", new { id = model.MaLSSN });
        }

        // ==================== EDIT ====================
        // GET: /Admin/LssnXa/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _lssnXaRepo.GetByIdAsync(id);
            if (item == null) return NotFound();

            await LoadXaDropDownsAsync(item.MaXaCu, item.MaXaMoi);

            var lssn = await _lssnRepo.GetByIdAsync(item.MaLSSN);
            ViewBag.LichSuSapNhap = lssn;

            return View(item);
        }

        // POST: /Admin/LssnXa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, LssnXa model)
        {
            if (id != model.MaLSSN) return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadXaDropDownsAsync(model.MaXaCu, model.MaXaMoi);
                var lssn = await _lssnRepo.GetByIdAsync(model.MaLSSN);
                ViewBag.LichSuSapNhap = lssn;

                return View(model);
            }

            await _lssnXaRepo.UpdateAsync(model);
            return RedirectToAction("Details", "LichSuSapNhap", new { id = model.MaLSSN });
        }

        // ==================== DELETE ====================
        // GET: /Admin/LssnXa/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _lssnXaRepo.GetByIdAsync(id);
            if (item == null) return NotFound();

            var lssn = await _lssnRepo.GetByIdAsync(item.MaLSSN);
            ViewBag.LichSuSapNhap = lssn;

            return View(item);
        }

        // POST: /Admin/LssnXa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var item = await _lssnXaRepo.GetByIdAsync(id);
            if (item == null) return NotFound();

            var maLssn = item.MaLSSN;

            await _lssnXaRepo.DeleteAsync(id);
            return RedirectToAction("Details", "LichSuSapNhap", new { id = maLssn });
        }

        // ==================== HELPER: LOAD DROPDOWN XÃ ====================
        private async Task LoadXaDropDownsAsync(int? selectedXaCu = null, int? selectedXaMoi = null)
        {
            var xaCuList = await _xaCuRepo.GetAllAsync();
            var xaMoiList = await _xaMoiRepo.GetAllAsync();

            ViewBag.XaCuList = new SelectList(xaCuList, "MaXaCu", "TenXaCu", selectedXaCu);
            ViewBag.XaMoiList = new SelectList(xaMoiList, "MaXaMoi", "TenXaMoi", selectedXaMoi);
        }
    }
}
