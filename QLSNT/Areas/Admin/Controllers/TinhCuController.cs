using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class TinhCuController : Controller
    {
        private readonly ITinhCuRepository _repo;

        public TinhCuController(ITinhCuRepository repo)
        {
            _repo = repo;
        }

        // GET: /Admin/TinhCu?search=Hà Nội
        public async Task<IActionResult> Index(string? search)
        {
            IEnumerable<TinhCu> list;

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

        // GET: /Admin/TinhCu/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _repo.GetByIdAsync(id.Value);

            if (item == null) return NotFound();

            // Tính tự động số lượng huyện và xã
            item.SLHuyen = item.HuyenCus?.Count ?? 0;
            item.SLXa = item.HuyenCus?.Sum(h => h.XaCus?.Count ?? 0) ?? 0;

            return View(item);
        }


        // GET: /Admin/TinhCu/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/TinhCu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TinhCu model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _repo.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/TinhCu/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _repo.GetByIdAsync(id.Value);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /Admin/TinhCu/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TinhCu model)
        {
            if (id != model.MaTinhCu) return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            await _repo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/TinhCu/Delete/1
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _repo.GetByIdAsync(id.Value);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /Admin/TinhCu/Delete/1
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
                .Select(x => x.TenTinhCu) // sửa theo field tên của bạn
                .Distinct()
                .Take(10)
                .ToList();

            return Json(result);
        }
    
    [HttpGet]
        public async Task<IActionResult> Suggest(string keyword)
        {
            var data = await _repo.SearchByNameAsync(keyword);

            return Json(data.Select(x => x.TenTinhCu).Take(10));
        }
    } 
}
