using Microsoft.AspNetCore.Mvc;
using QLSNT.Models;
using QLSNT.Repositories;
using Microsoft.AspNetCore.Authorization;
namespace QLSNT.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class HuyenCuController : Controller
    {
        private readonly IHuyenCuRepository _repo;
        private readonly ITinhCuRepository _repoTinhCu;

        public HuyenCuController(IHuyenCuRepository repo, ITinhCuRepository tinhCu)
        {
            _repo = repo;
            _repoTinhCu = tinhCu;
        }

        // GET: /HuyenCu?search=Quận 1
        public async Task<IActionResult> Index(string? search)
        {
            IEnumerable<HuyenCu> list;

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

        // GET: /HuyenCu/Details/H001
        public async Task<IActionResult> Details(int id)  // Chuyển id từ string sang int
        {
            if (id == 0)
                return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // GET: /HuyenCu/Create
        // GET: /HuyenCu/Create
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách tỉnh để hiển thị dropdown
            var tinhCus = await _repoTinhCu.GetAllAsync();
            ViewBag.TinhCus = tinhCus;

            return View();
        }

        // POST: /HuyenCu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HuyenCu model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu ModelState invalid thì phải gán lại ViewBag để dropdown không bị null
                var tinhCus = await _repoTinhCu.GetAllAsync();
                ViewBag.TinhCus = tinhCus;
                return View(model);
            }

            await _repo.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }


        // GET: /HuyenCu/Edit/H001
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var huyen = await _repo.GetByIdAsync(id.Value);
            if (huyen == null) return NotFound();

            // Lấy danh sách tỉnh để hiển thị dropdown
            var tinhCus = await _repoTinhCu.GetAllAsync();
            ViewBag.TinhCus = tinhCus;

            return View(huyen);
        }


        // POST: /HuyenCu/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HuyenCu model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _repo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /HuyenCu/Delete/H001
        public async Task<IActionResult> Delete(int id)  // Chuyển id từ string sang int
        {
            if (id == 0)
                return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /HuyenCu/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)  // Chuyển id từ string sang int
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetByTinhCu(int maTinh)
        {
            var huyens = await _repo.GetByTinhCuAsync(maTinh);

            // map sang JSON với field đúng như script đang đọc
            var result = huyens.Select(h => new {
                maHuyen = h.MaHuyenCu,
                tenHuyen = h.TenHuyenCu
            });

            return Json(result);   // đây mới là đoạn trả về JSON
        }
        [HttpGet]
        public async Task<IActionResult> Autocomplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var data = await _repo.SearchByNameAsync(term);

            var result = data
                .Select(x => x.TenHuyenCu) // sửa theo field tên của bạn
                .Distinct()
                .Take(10)
                .ToList();

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> Suggest(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Json(new List<string>());

            var data = await _repo.SearchByNameAsync(keyword);

            var result = data
                .Where(x => x != null && !string.IsNullOrEmpty(x.TenHuyenCu))
                .Select(x => x.TenHuyenCu.Trim())
                .Where(x => x != "")
                .Distinct()
                .Take(5)
                .ToList();

            return Json(result);
        }

    }
}
