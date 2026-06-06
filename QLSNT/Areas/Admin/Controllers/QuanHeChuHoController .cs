using Microsoft.AspNetCore.Mvc;
using QLSNT.Models;
using QLSNT.Repositories;
using Microsoft.AspNetCore.Authorization;
namespace QLSNT.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class QuanHeChuHoController : Controller
    {
        private readonly IQuanHeChuHoRepository _repo;

        public QuanHeChuHoController(IQuanHeChuHoRepository repo)
        {
            _repo = repo;
        }

        // GET: /QuanHeChuHo?search=Con
        public async Task<IActionResult> Index(string? search)
        {
            IEnumerable<QuanHeChuHo> list;

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

        // GET: /QuanHeChuHo/Details/QH01
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // GET: /QuanHeChuHo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /QuanHeChuHo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(QuanHeChuHo model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Lấy danh sách để tìm mã cuối cùng
            var all = await _repo.GetAllAsync();
            var last = all.OrderByDescending(q => q.MaQHCH).FirstOrDefault();

            int nextId = 1;
            if (last != null)
            {
                // giả sử mã dạng QHCH001
                string lastCode = last.MaQHCH;
                nextId = int.Parse(lastCode.Substring(4)) + 1;
            }

            model.MaQHCH = $"QHCH{nextId:D3}"; // QHCH001, QHCH002...

            await _repo.AddAsync(model);
            await _repo.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: /QuanHeChuHo/Edit/QH01
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /QuanHeChuHo/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuanHeChuHo model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _repo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /QuanHeChuHo/Delete/QH01
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /QuanHeChuHo/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
