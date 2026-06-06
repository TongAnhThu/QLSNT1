using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLSNT.Data;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]  
    [Authorize(Roles = "Admin,Employee")]
    public class NguoiDanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INguoiDanRepository _nguoiDanRepo;
        private readonly IDanTocRepository _danTocRepo;
        private readonly ITonGiaoRepository _tonGiaoRepo;
        private readonly ITrinhDoVanHoaRepository _trinhDoRepo;
        private readonly IQuanHeChuHoRepository _quanHeChuHoRepo;

        public NguoiDanController(
            ApplicationDbContext context,
            INguoiDanRepository nguoiDanRepo,
            IDanTocRepository danTocRepo,
            ITonGiaoRepository tonGiaoRepo,
            ITrinhDoVanHoaRepository trinhDoRepo,
            IQuanHeChuHoRepository quanHeChuHoRepo)
        {
            _context = context;
            _nguoiDanRepo = nguoiDanRepo;
            _danTocRepo = danTocRepo;
            _tonGiaoRepo = tonGiaoRepo;
            _trinhDoRepo = trinhDoRepo;
            _quanHeChuHoRepo = quanHeChuHoRepo;
        }

        // =========================
        // Helper: load dropdowns
        // =========================
        private async Task LoadDropdownsAsync()
        {
            var danTocs = await _danTocRepo.GetAllAsync();
            var tonGiaos = await _tonGiaoRepo.GetAllAsync();
            var trinhDos = await _trinhDoRepo.GetAllAsync();
            var quanHeChuHos = await _quanHeChuHoRepo.GetAllAsync();

            ViewBag.DanTocList = new SelectList(danTocs, "MaDanToc", "TenDanToc");
            ViewBag.TonGiaoList = new SelectList(tonGiaos, "MaTonGiao", "TenTonGiao");
            ViewBag.TrinhDoList = new SelectList(trinhDos, "MaTrinhDo", "TenTrinhDo");
            ViewBag.QuanHeChuHoList = new SelectList(quanHeChuHos, "MaQuanHe", "TenQuanHe");
        }

        // =========================
        // GET: /Admin/NguoiDan
        // =========================
        public async Task<IActionResult> Index(string? keyword)
        {
            IEnumerable<NguoiDan> list;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = await _nguoiDanRepo.SearchAsync(keyword);
                ViewBag.Keyword = keyword;
            }
            else
            {
                list = await _nguoiDanRepo.GetAllAsync();
            }

            return View(list);
        }

        // =========================
        // GET: /Admin/NguoiDan/Details/0123456789
        // =========================
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var item = await _nguoiDanRepo.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // =========================
        // GET: /Admin/NguoiDan/Create
        // =========================
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View();
        }

        // POST: /Admin/NguoiDan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NguoiDan model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            await _nguoiDanRepo.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // GET: /Admin/NguoiDan/Edit/0123456789
        // =========================
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var item = await _nguoiDanRepo.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            await LoadDropdownsAsync();
            return View(item);
        }

        // POST: /Admin/NguoiDan/Edit/...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NguoiDan model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            await _nguoiDanRepo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // GET: /Admin/NguoiDan/Delete/0123456789
        // =========================
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var item = await _nguoiDanRepo.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /Admin/NguoiDan/Delete/0123456789
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                await _nguoiDanRepo.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Suggest(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Json(new List<string>());

            var data = await _context.NguoiDans
                .Where(x => x.HoTen.Contains(keyword))
                .Select(x => x.HoTen)
                .Take(5) // giới hạn 5 gợi ý
                .ToListAsync();

            return Json(data);
        }
    }
}
