using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLSNT.Repositories;

namespace QLSNT.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class TamTruController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly INguoiDanRepository _nguoiDanRepository;
        private readonly ITamTruRepository _tamTruRepository;

    public TamTruController(
        UserManager<IdentityUser> userManager,
        INguoiDanRepository nguoiDanRepository,
        ITamTruRepository tamTruRepository)
        {
            _userManager = userManager;
            _nguoiDanRepository = nguoiDanRepository;
            _tamTruRepository = tamTruRepository;
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

            var tamTruList = await _tamTruRepository
                .GetTamTruHieuLucByNguoiDanIdAsync(nguoiDan.MaCCCD);

            return View(tamTruList);
        }
    }


}
