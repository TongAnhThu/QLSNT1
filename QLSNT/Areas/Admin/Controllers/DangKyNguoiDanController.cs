using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLSNT.Models;
using QLSNT.ViewModels;
using QLSNT.Repositories;
using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;


namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DangKyNguoiDanController : Controller
    {
        private readonly INguoiDanRepository _nguoiDanRepo;
        private readonly IThuongTruRepository _thuongTruRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDanTocRepository _danTocRepo;
        private readonly ITonGiaoRepository _tonGiaoRepo;
        private readonly ITrinhDoVanHoaRepository _trinhDoRepo;
        private readonly IQuanHeChuHoRepository _quanHeChuHoRepo;
        private readonly ITinhMoiRepository _tinhRepo;
        private readonly IXaMoiRepository _xaMoiRepo;
        private readonly ILichSuDiaChiRepository _lichSuDiaChiRepo;
        public DangKyNguoiDanController(
            INguoiDanRepository nguoiDanRepo,
            IThuongTruRepository thuongTruRepo,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IDanTocRepository danTocRepo,
            ITonGiaoRepository tonGiaoRepo,
            ITrinhDoVanHoaRepository trinhDoRepo,
            IQuanHeChuHoRepository quanHeChuHoRepo,
            ITinhMoiRepository tinhRepo,
            IXaMoiRepository xaMoiRepo,
            ILichSuDiaChiRepository lichSuDiaChiRepo)
        {
            _nguoiDanRepo = nguoiDanRepo;
            _thuongTruRepo = thuongTruRepo;
            _userManager = userManager;
            _roleManager = roleManager;
            _danTocRepo = danTocRepo;
            _tonGiaoRepo = tonGiaoRepo;
            _trinhDoRepo = trinhDoRepo;
            _quanHeChuHoRepo = quanHeChuHoRepo;
            _tinhRepo = tinhRepo;
            _xaMoiRepo = xaMoiRepo;
            _lichSuDiaChiRepo = lichSuDiaChiRepo;
        }
        // ============================
        // Load dropdowns cho view
        // ============================
        private async Task LoadDropdownsAsync()
        {
            ViewBag.DanTocList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _danTocRepo.GetAllAsync(), "MaDanToc", "TenDanToc");
            ViewBag.TonGiaoList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _tonGiaoRepo.GetAllAsync(), "MaTonGiao", "TenTonGiao");
            ViewBag.TrinhDoList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _trinhDoRepo.GetAllAsync(), "MaTDVH", "TenTDVH");
            ViewBag.QuanHeChuHoList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _quanHeChuHoRepo.GetAllAsync(), "MaQHCH", "TenQHCH");
            ViewBag.TinhList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _tinhRepo.GetAllAsync(), "MaTinhMoi", "TenTinhMoi");
            ViewBag.XaList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _xaMoiRepo.GetAllAsync(), "MaXaMoi", "TenXaMoi");

            ViewBag.TinhTrangHonNhanList = new[]
            {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Độc thân", "Độc thân"),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Đã kết hôn", "Đã kết hôn"),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Ly hôn", "Ly hôn"),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Góa", "Góa")
            };

            ViewBag.TrangThaiCongDanList = new[]
            {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Còn sống", "Còn sống"),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Đã mất", "Đã mất"),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Mất tích", "Mất tích")
            };
        }

        // ============================
        // BƯỚC 1: THÔNG TIN CƠ BẢN
        // ============================
        public string GenerateCCCD(DateTime ngaySinh, string gioiTinh, string maTinh)
        {
            string ppp = maTinh.PadLeft(3, '0');

            int year = ngaySinh.Year;
            string g;

            if (gioiTinh == "Nam")
                g = (year < 2000) ? "0" : "2";
            else
                g = (year < 2000) ? "1" : "3";

            string yy = (year % 100).ToString("D2");

            var random = new Random();
            string nnnnnn = random.Next(0, 999999).ToString("D6");

            return $"{ppp}{g}{yy}{nnnnnn}";
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View("Create", new NguoiDan());
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            NguoiDan nguoiDan,
            int MaXaMoi,
            string DiaChiThuongTru
        )
        {
            ModelState.Remove("MaCCCD");

            // Validate địa chỉ
            if (MaXaMoi == 0)
                ModelState.AddModelError("", "Vui lòng chọn xã");

            if (string.IsNullOrEmpty(DiaChiThuongTru))
                ModelState.AddModelError("", "Vui lòng nhập địa chỉ");

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View("Create", nguoiDan);
            }

            // Chuẩn hóa tên
            nguoiDan.HoTenKhongDau = RemoveDiacritics(nguoiDan.HoTen);

            // 1. Lấy xã → tỉnh
            var xa = await _xaMoiRepo.GetByIdAsync(MaXaMoi);
            string maTinh = xa.TinhMoi.MaTinhMoi.ToString("D3");

            // 2. Sinh CCCD
            string newCCCD;
            do
            {
                newCCCD = GenerateCCCD(
                    nguoiDan.NgaySinh.Value,
                    nguoiDan.GioiTinh,
                    maTinh
                );
            }
            while (await _nguoiDanRepo.ExistsAsync(newCCCD));

            nguoiDan.MaCCCD = newCCCD;

            // 3. Lưu người dân
            await _nguoiDanRepo.AddAsync(nguoiDan);

            // 4. Lưu thường trú
            var thuongTru = new ThuongTru
            {
                MaCCCD = newCCCD,
                MaXaMoi = MaXaMoi,
                DiaChi = DiaChiThuongTru,
                NgayDangKy = DateTime.Today
            };

            await _thuongTruRepo.AddAsync(thuongTru);

            // 5. Tạo user
            var user = new IdentityUser
            {
                UserName = newCCCD,
                Email = $"{newCCCD}@qlsnt.local",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, newCCCD);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                await LoadDropdownsAsync();
                return View("Create", nguoiDan);
            }

            // 6. Role
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _userManager.AddToRoleAsync(user, "User");

            TempData["SuccessMessage"] = $"Tạo thành công CCCD: {newCCCD}";
            return RedirectToAction("Index", "NguoiDan", new { area = "Admin" });
        }


        // ============================
        // EDIT THÔNG TIN CƠ BẢN
        // ============================
        [HttpGet]
        public async Task<IActionResult> Edit(string maCCCD)
        {
            if (string.IsNullOrWhiteSpace(maCCCD))
                return RedirectToAction("Index", "NguoiDan");

            var nguoiDan = await _nguoiDanRepo.GetByIdAsync(maCCCD);
            if (nguoiDan == null)
                return NotFound();

            await LoadDropdownsAsync();
            return View("EditBasic", nguoiDan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NguoiDan nguoiDan)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View("EditBasic", nguoiDan);
            }

            await _nguoiDanRepo.UpdateAsync(nguoiDan);

            TempData["SuccessMessage"] = "Cập nhật thông tin cơ bản thành công!";
            return RedirectToAction("Index", "NguoiDan", new { area = "Admin" });
        }
        // ============================
        // EDIT ĐỊA CHỈ THƯỜNG TRÚ
        // ============================
        [HttpGet]
        public async Task<IActionResult> EditDetails(string maCCCD)
        {
            if (string.IsNullOrWhiteSpace(maCCCD))
                return RedirectToAction("Index", "NguoiDan");

            // Nếu repo chưa có GetByCCCDAsync, bạn có thể viết thêm hoặc dùng SearchAsync
            var thuongTru = await _thuongTruRepo.GetByCCCDAsync(maCCCD);
            if (thuongTru == null)
                return NotFound();

            var model = new NguoiDanCreateViewModel
            {
                MaCCCD = thuongTru.MaCCCD,
                MaXaMoi = thuongTru.MaXaMoi,
                DiaChiThuongTru = thuongTru.DiaChi
            };

            await LoadDropdownsAsync();
            return View("EditDetails", model);
        }

        [HttpPost]

        public async Task<IActionResult> EditDetails(NguoiDanCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View("EditDetails", model);
            }

            var thuongTru = await _thuongTruRepo.GetByCCCDAsync(model.MaCCCD);
            if (thuongTru == null)
                return NotFound();

            // Nếu địa chỉ thay đổi thì ghi lịch sử
            if (thuongTru.DiaChi != model.DiaChiThuongTru || thuongTru.MaXaMoi != model.MaXaMoi)
            {
                var lichSu = new LichSuDiaChi
                {
                    MaLichSuCuTru = await GenerateMaLichSuCuTruAsync(),
                    MaCCCD = thuongTru.MaCCCD,
                    DiaChiCu = thuongTru.DiaChi,
                    DiaChiMoi = model.DiaChiThuongTru,
                    NgayHieuLuc = DateTime.UtcNow,
                    NguoiTao = User.Identity?.Name,
                    NgayTao = DateTime.Now,

                    // Chỉ còn mã xã mới
                    MaXaMoi = model.MaXaMoi
                };
                await _lichSuDiaChiRepo.AddAsync(lichSu);
            }

            // Cập nhật thông tin mới
            thuongTru.MaXaMoi = model.MaXaMoi;
            thuongTru.DiaChi = model.DiaChiThuongTru;
            await _thuongTruRepo.UpdateAsync(thuongTru);

            return RedirectToAction("Index", "NguoiDan");
        }


        // ============================
        // XEM CHI TIẾT NGƯỜI DÂN
        // ============================
        [HttpGet]
        public async Task<IActionResult> Details(string maCCCD)
        {
            if (string.IsNullOrWhiteSpace(maCCCD))
                return RedirectToAction("Index", "NguoiDan");

            var nguoiDan = await _nguoiDanRepo.GetByIdAsync(maCCCD);
            if (nguoiDan == null)
                return NotFound();

            var thuongTru = await _thuongTruRepo.GetByCCCDAsync(maCCCD); // cần implement nếu chưa có

            var model = new NguoiDanDetailsViewModel
            {
                MaCCCD = nguoiDan.MaCCCD,
                MaXaMoi = thuongTru?.MaXaMoi,
                DiaChiThuongTru = thuongTru?.DiaChi,
                HoTen = nguoiDan.HoTen,
                HoTenKhongDau = nguoiDan.HoTenKhongDau,   // gán thêm ở đây
                NgaySinh = nguoiDan.NgaySinh,
                GioiTinh = nguoiDan.GioiTinh,
                MaDanToc = nguoiDan.MaDanToc,
                MaTonGiao = nguoiDan.MaTonGiao,
                MaTDVH = nguoiDan.MaTDVH,
                TinhTrangHonNhan = nguoiDan.TinhTrangHonNhan,
                TrangThaiCongDan = nguoiDan.TrangThaiCongDan
            };



            await LoadDropdownsAsync();
            return View("Details", model);
        }
        [HttpGet]
        public async Task<JsonResult> GetXaByTinh(int maTinh)
        {
            var xaList = await _xaMoiRepo.GetByTinhAsync(maTinh);

            return Json(xaList.Select(x => new
            {
                id = x.MaXaMoi,
                name = x.TenXaMoi
            }));
        }
        private async Task<string> GenerateMaLichSuCuTruAsync()
        {
            var last = await _lichSuDiaChiRepo.GetLastCodeAsync(); // ví dụ lấy mã cuối cùng
            int number = 0;
            if (!string.IsNullOrEmpty(last) && last.Length > 2)
            {
                int.TryParse(last.Substring(2), out number);
            }
            return $"LS{(number + 1):D3}";
        }


        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

       


    }
}