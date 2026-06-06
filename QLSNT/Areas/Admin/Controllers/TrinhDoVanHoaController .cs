using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class TrinhDoVanHoaController : Controller
    {
        private readonly ITrinhDoVanHoaRepository _repo;

        public TrinhDoVanHoaController(ITrinhDoVanHoaRepository repo)
        {
            _repo = repo;
        }

        // GET: /TrinhDoVanHoa?search=12/12
        public async Task<IActionResult> Index(string? search)
        {
            IEnumerable<TrinhDoVanHoa> list;

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

        // GET: /TrinhDoVanHoa/Details/01
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // GET: /TrinhDoVanHoa/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrinhDoVanHoa model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Lấy danh sách để tìm mã cuối cùng
            var all = await _repo.GetAllAsync();
            var last = all.OrderByDescending(t => t.MaTDVH).FirstOrDefault();

            int nextId = 1;
            if (last != null)
            {
                // giả sử mã dạng TDVH001
                string lastCode = last.MaTDVH;
                nextId = int.Parse(lastCode.Substring(4)) + 1;
            }

            model.MaTDVH = $"TDVH{nextId:D3}";
            await _repo.AddAsync(model);
            await _repo.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: /TrinhDoVanHoa/Edit/01
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /TrinhDoVanHoa/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TrinhDoVanHoa model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _repo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /TrinhDoVanHoa/Delete/01
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /TrinhDoVanHoa/Delete/01
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
                .Select(x => x.TenTDVH) // sửa theo field tên của bạn
                .Distinct()
                .Take(10)
                .ToList();

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> Suggest(string keyword)
        {
            var data = await _repo.SearchByNameAsync(keyword);

            return Json(data.Select(x => x.TenTDVH).Take(10));
        }
    }
}
