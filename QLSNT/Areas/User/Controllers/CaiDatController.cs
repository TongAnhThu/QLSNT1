using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace QLSNT.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class CaiDatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Chuyển sang trang đổi mật khẩu của Identity
        public IActionResult DoiMatKhau()
        {
            return Redirect("/Identity/Account/Manage/ChangePassword");
        }

        [HttpPost]
        public IActionResult DoiNgonNgu(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });

            return RedirectToAction(nameof(Index));
        
        }
       

    }
}


