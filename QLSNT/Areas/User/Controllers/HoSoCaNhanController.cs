using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLSNT.Areas.User.ViewModel;
using QLSNT.Repositories;

namespace QLSNT.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class ThongTinCaNhanController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly INguoiDanRepository _nguoiDanRepository;
        private readonly IThuongTruRepository _thuongTruRepository;
        private readonly ITamTruRepository _tamTruRepository;

        public ThongTinCaNhanController(
            UserManager<IdentityUser> userManager,
            INguoiDanRepository nguoiDanRepository,
            IThuongTruRepository thuongTruRepository,
            ITamTruRepository tamTruRepository)
        {
            _userManager = userManager;
            _nguoiDanRepository = nguoiDanRepository;
            _thuongTruRepository = thuongTruRepository;
            _tamTruRepository = tamTruRepository;
        }

        /// <summary>
        /// Người dân xem hồ sơ cá nhân
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy tài khoản đang đăng nhập
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Challenge();
                }

                // UserName = CCCD
                var nguoiDan = await _nguoiDanRepository
                    .GetByIdentityUserIdAsync(user.UserName);

                if (nguoiDan == null)
                {
                    ViewBag.Message = "Không tìm thấy thông tin công dân.";
                    return View();
                }

                // Thường trú hiện tại
                var thuongTru = await _thuongTruRepository
                    .GetThuongTruHienTaiByNguoiDanIdAsync(nguoiDan.MaCCCD);

                // Danh sách tạm trú còn hiệu lực
                var tamTruList = await _tamTruRepository
                    .GetTamTruHieuLucByNguoiDanIdAsync(nguoiDan.MaCCCD);

                var model = new HoSoCaNhanViewModel
                {
                    NguoiDan = nguoiDan,
                    ThuongTruHienTai = thuongTru,
                    DanhSachTamTru = tamTruList
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xảy ra: {ex.Message}";
                return View();
            }
        }
    }
}