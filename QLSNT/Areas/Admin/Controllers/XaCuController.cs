using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class XaCuController : Controller
    {
        private readonly IXaCuRepository _repoXa;
        private readonly IHuyenCuRepository _repoHuyen;

        public XaCuController(IXaCuRepository repoXa, IHuyenCuRepository repoHuyen)
        {
            _repoXa = repoXa;
            _repoHuyen = repoHuyen;
        }

        // GET: /Admin/XaCu?search=Thảo Điền
        public async Task<IActionResult> Index(string? search)
        {
            IEnumerable<XaCu> list;

            if (!string.IsNullOrWhiteSpace(search))
            {
                list = await _repoXa.SearchByNameAsync(search);
                ViewBag.Search = search;
            }
            else
            {
                list = await _repoXa.GetAllAsync();
            }

            // include huyện và tỉnh để hiển thị tên
            foreach (var xa in list)
            {
                if (xa.HuyenCu != null)
                {
                    xa.HuyenCu = await _repoHuyen.GetByIdAsync(xa.MaHuyenCu);
                }
            }

            return View(list);
        }

        // GET: /Admin/XaCu/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _repoXa.GetByIdAsync(id.Value);
            if (item == null) return NotFound();

            // load huyện và tỉnh
            item.HuyenCu = await _repoHuyen.GetByIdAsync(item.MaHuyenCu);

            return View(item);
        }

        // GET: /Admin/XaCu/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var huyenCus = await _repoHuyen.GetAllAsync();
            ViewBag.HuyenCuList = huyenCus;
            return View();
        }

        // POST: /Admin/XaCu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(XaCu model)
        {
            if (!ModelState.IsValid)
            {
                var huyenCus = await _repoHuyen.GetAllAsync();
                ViewBag.HuyenCuList = huyenCus;
                return View(model);
            }

            await _repoXa.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/XaCu/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var xa = await _repoXa.GetByIdAsync(id);
            if (xa == null) return NotFound();

            var huyenCus = await _repoHuyen.GetAllAsync();
            ViewBag.HuyenCuList = huyenCus;

            return View(xa);
        }

        // POST: /Admin/XaCu/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, XaCu model)
        {
            if (id != model.MaXaCu) return BadRequest();

            if (!ModelState.IsValid)
            {
                var huyenCus = await _repoHuyen.GetAllAsync();
                ViewBag.HuyenCuList = huyenCus;
                return View(model);
            }

            await _repoXa.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/XaCu/Delete/1
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _repoXa.GetByIdAsync(id.Value);
            if (item == null) return NotFound();

            item.HuyenCu = await _repoHuyen.GetByIdAsync(item.MaHuyenCu);

            return View(item);
        }

        // POST: /Admin/XaCu/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repoXa.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
       

        [HttpGet]
        public async Task<IActionResult> GetByHuyen(int maHuyen)
        {
            var xas = await _repoXa.GetByHuyenAsync(maHuyen);
            var result = xas.Select(x => new {
                maXaCu = x.MaXaCu,
                tenXaCu = x.TenXaCu
            });
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> Autocomplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var data = await _repoXa.SearchByNameAsync(term);

            var result = data
                .Select(x => x.TenXaCu) // sửa theo field tên của bạn
                .Distinct()
                .Take(10)
                .ToList();

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> Suggest(string keyword)
        {
            var data = await _repoXa.SearchByNameAsync(keyword);

            return Json(data.Select(x => x.TenXaCu).Take(10));
        }
    }
}
