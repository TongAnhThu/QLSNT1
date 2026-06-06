using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class TonGiaoController : Controller
    {
        private readonly ITonGiaoRepository _repo;

        public TonGiaoController(ITonGiaoRepository repo)
        {
            _repo = repo;
        }

        // GET: /TonGiao?search=Phật
        public async Task<IActionResult> Index(string? search)
        {
            IEnumerable<TonGiao> list;

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

        // GET: /TonGiao/Details/TG01
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TonGiao model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Lấy tất cả để tìm mã cuối cùng
            var all = await _repo.GetAllAsync();
            var last = all.OrderByDescending(t => t.MaTonGiao).FirstOrDefault();

            int nextId = 1;
            if (last != null)
            {
                // giả sử mã dạng TG001
                string lastCode = last.MaTonGiao;
                nextId = int.Parse(lastCode.Substring(2)) + 1;
            }

            model.MaTonGiao = $"TG{nextId:D3}"; // TG001, TG002...

            await _repo.AddAsync(model);
            await _repo.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: /TonGiao/Edit/TG01
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /TonGiao/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TonGiao model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _repo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /TonGiao/Delete/TG01
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /TonGiao/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
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
                .Select(x => x.TenTonGiao) // sửa theo field tên của bạn
                .Distinct()
                .Take(10)
                .ToList();

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> Suggest(string keyword)
        {
            var data = await _repo.SearchByNameAsync(keyword);

            return Json(data.Select(x => x.TenTonGiao).Take(10));
        }
    }
}
