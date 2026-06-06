using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class TinhMoiController : Controller
    {
        private readonly ITinhMoiRepository _repo;

        public TinhMoiController(ITinhMoiRepository repo)
        {
            _repo = repo;
        }

        // GET: /Admin/TinhMoi?search=Hà Nội
        public async Task<IActionResult> Index(string? search)
        {
            IEnumerable<TinhMoi> list;

            if (!string.IsNullOrWhiteSpace(search))
            {
                list = await _repo.SearchByNameAsync(search);
                ViewBag.Search = search;
            }
            else
            {
                list = await _repo.GetAllAsync();
            }

            return View(list);
        }

        // GET: /Admin/TinhMoi/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _repo.GetByIdAsync(id.Value);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // GET: /Admin/TinhMoi/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/TinhMoi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TinhMoi model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _repo.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/TinhMoi/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _repo.GetByIdAsync(id.Value);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /Admin/TinhMoi/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TinhMoi model)
        {
            if (id != model.MaTinhMoi)   // đảm bảo route id khớp model
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            await _repo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/TinhMoi/Delete/1
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _repo.GetByIdAsync(id.Value);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /Admin/TinhMoi/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Autocomplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var data = await _repo.SearchByNameAsync(term);

            var result = data
                .Select(x => x.TenTinhMoi) // sửa theo field tên của bạn
                .Distinct()
                .Take(10)
                .ToList();

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> Suggest(string keyword)
        {
            var data = await _repo.SearchByNameAsync(keyword);

            return Json(data.Select(x => x.TenTinhMoi).Take(10));
        }
    }
}
