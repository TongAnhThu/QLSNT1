// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLSNT.Data;
using QLSNT.Models;

namespace QLSNT.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LogoutModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LogoutModel> logger,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            var userId = _userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(userId))
            {
                _context.NhatKyHoatDongs.Add(new NhatKyHoatDong
                {
                    UserId = userId,
                    HanhDong = "Đăng xuất",
                    MoTa = "Người dùng đăng xuất khỏi hệ thống",
                    ThoiGian = DateTime.Now,
                    DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                await _context.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();

            return Redirect("/Identity/Account/Login");
        }
    }
}
