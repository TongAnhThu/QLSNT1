using Microsoft.AspNetCore.Mvc;
using QLSNT.Models;
using QLSNT.Repositories;
using Microsoft.AspNetCore.Authorization;
namespace QLSNT.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class SuKienHanhChinhController : Controller
    {
        private readonly ISuKienHanhChinhRepository _repo;

        public SuKienHanhChinhController(ISuKienHanhChinhRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index(string? search)
        {
            IEnumerable<SuKienHanhChinh> list;

            if (!string.IsNullOrWhiteSpace(search))
            {
                list = await _repo.SearchAsync(search);
                ViewBag.Search = search;
            }
            else
            {
                list = await _repo.GetAllAsync();
            }

            return View(list);
        }

        // GET: /SuKienHanhChinh/Details/123-2020-NĐ-CP
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SuKienHanhChinh model)
        {
            if (!ModelState.IsValid)
            {
                // nếu view có dropdown thì nhớ load lại ở đây (nếu có)
                return View(model);
            }

            // Gán người tạo + thời gian tạo
            model.NguoiTao = User.Identity?.Name;
            model.NgayTao = DateTime.Now;

            await _repo.AddAsync(model);

            TempData["SuccessMessage"] = "Thêm sự kiện hành chính thành công.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SuKienHanhChinh model)
        {
            if (id != model.SoNghiDinh) return NotFound();

            if (!ModelState.IsValid)
            {
                // load dropdown nếu có
                return View(model);
            }

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            // map các trường cho phép sửa
            entity.TenSK = model.TenSK;
            entity.NgayBanHanh = model.NgayBanHanh;
            
            entity.MoTaChiTiet = model.MoTaChiTiet;
            // ... các trường khác ...

            // thông tin cập nhật
            entity.NguoiTao = User.Identity?.Name;
            entity.NgayTao = DateTime.Now;

            await _repo.UpdateAsync(entity);

            TempData["SuccessMessage"] = "Cập nhật sự kiện hành chính thành công.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
