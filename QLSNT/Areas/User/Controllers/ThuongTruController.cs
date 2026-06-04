using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLSNT.Models;
using QLSNT.Repositories;

namespace QLSNT.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class ThuongTruController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly INguoiDanRepository _nguoiDanRepository;
        private readonly IThuongTruRepository _thuongTruRepository;


    public ThuongTruController(
        UserManager<IdentityUser> userManager,
        INguoiDanRepository nguoiDanRepository,
        IThuongTruRepository thuongTruRepository)
        {
            _userManager = userManager;
            _nguoiDanRepository = nguoiDanRepository;
            _thuongTruRepository = thuongTruRepository;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var nguoiDan = await _nguoiDanRepository
                .GetByIdentityUserIdAsync(user.UserName);

            if (nguoiDan == null)
                return NotFound();

            var thuongTru = await _thuongTruRepository
                .GetThuongTruHienTaiByNguoiDanIdAsync(nguoiDan.MaCCCD);

            return View(thuongTru);
        }

    }


}
