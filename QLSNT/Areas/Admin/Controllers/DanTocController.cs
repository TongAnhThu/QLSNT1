using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSNT.Models;
using QLSNT.Repositories;
using QLSNT.Data;
using Microsoft.AspNetCore.Authorization;
namespace QLSNT.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class DanTocController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDanTocRepository _danTocRepo;

        public DanTocController(IDanTocRepository danTocRepo, ApplicationDbContext context)
        {
            _context = context;
            _danTocRepo = danTocRepo;
        }

        // GET: /DanToc
        // Có hỗ trợ tìm kiếm theo tên (?search=Kinh)
        public async Task<IActionResult> Index(string? search)
        {
            IEnumerable<DanToc> list;

            if (!string.IsNullOrWhiteSpace(search))
            {
                list = await _danTocRepo.SearchByNameAsync(search);
                ViewBag.Search = search;
            }
            else
            {
                list = await _danTocRepo.GetAllAsync();
            }

            return View(list);
        }

        // GET: /DanToc/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _danTocRepo.GetByIdAsync(id.Value);
            if (item == null) return NotFound();

            return View(item);
        }

        // GET: /DanToc/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /DanToc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DanToc model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // KHÔNG gán MaDanToc, DB sẽ tự sinh
            // Chỉ cần gán các trường khác
            var danToc = new DanToc
            {
                TenDanToc = model.TenDanToc,
                TenKhac = model.TenKhac,
                MoTa = model.MoTa,
                GhiChu = model.GhiChu
            };

            await _danTocRepo.AddAsync(danToc);
            await _danTocRepo.SaveChangesAsync();

            // Sau khi SaveChanges, EF sẽ tự cập nhật MaDanToc vừa sinh vào object danToc
            int newId = danToc.MaDanToc; // lấy mã dân tộc mới

            return RedirectToAction(nameof(Index));
        }


        // GET: /DanToc/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _danTocRepo.GetByIdAsync(id.Value);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /DanToc/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DanToc model)
        {
            if (id != model.MaDanToc) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _danTocRepo.UpdateAsync(model);
            await _danTocRepo.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /DanToc/Delete/1
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _danTocRepo.GetByIdAsync(id.Value);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /DanToc/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _danTocRepo.DeleteAsync(id);
            await _danTocRepo.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Suggest(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Json(new List<string>());

            var data = await _context.DanTocs
                .Where(x => x.TenDanToc.Contains(keyword))
                .Select(x => x.TenDanToc)
                .Take(5)
                .ToListAsync();

            return Json(data);
        }
    }
}
