using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using QLSNT.Data;
using QLSNT.Models;

namespace QLSNT.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;

        public LoginModel(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          ILogger<LoginModel> logger,
                          ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Mã Căn Cước Công Dân")]
            public string MaCCCD { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; }

            [Display(Name = "Ghi nhớ đăng nhập")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins =
                (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                Input.MaCCCD,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("Người dùng đăng nhập thành công.");

                var user = await _userManager.FindByNameAsync(Input.MaCCCD);

                if (user != null)
                {
                    _context.NhatKyHoatDongs.Add(new NhatKyHoatDong
                    {
                        UserId = user.Id,
                        HanhDong = "Đăng nhập",
                        MoTa = $"Đăng nhập hệ thống bằng CCCD {Input.MaCCCD}",
                        ThoiGian = DateTime.Now,
                        DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
                    });

                    await _context.SaveChangesAsync();

                    // Nếu có returnUrl thì ưu tiên chuyển theo returnUrl
                    if (!string.IsNullOrEmpty(returnUrl) &&
                        returnUrl != "/")
                    {
                        return LocalRedirect(returnUrl);
                    }

                    // Admin
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return Redirect("/Admin/NguoiDan");
                    }

                    // User
                    return Redirect("/Home");
                }
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage(
                    "./LoginWith2fa",
                    new
                    {
                        ReturnUrl = returnUrl,
                        RememberMe = Input.RememberMe
                    });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Tài khoản bị khóa.");

                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(
                string.Empty,
                "Đăng nhập không hợp lệ.");

            return Page();
        }
    }
}
